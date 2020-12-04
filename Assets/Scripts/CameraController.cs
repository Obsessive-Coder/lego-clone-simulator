using UnityEngine;

public class CameraController : MonoBehaviour {
  private Camera mainCamera;
  private CursorController cursor;
  private EdgeAxes edgeAxes;
  private Vector3 movement = Vector3.zero;
  private Vector3 rotation = Vector3.zero;

  public bool isEdgeScrollEnabled = true;
  public float edgeBoundary = 50.0f;

  public float movementSpeed = 10.0f;
  public float MovementSpeed {
    get { return movementSpeed * Time.deltaTime; }
    set { movementSpeed = value; }
  }

  public float rotationSpeed = 100.0f;
  public float RotationSpeed {
    get { return rotationSpeed * Time.deltaTime; }
    set { rotationSpeed = value; }
  }

  public float zoomSpeed = 1.0f;
  public float ZoomSpeed {
    get { return zoomSpeed * Time.deltaTime; }
    set { zoomSpeed = value; }
  }

  void Start() {
    mainCamera = transform.GetChild(0).gameObject.GetComponent<Camera>();
    cursor = gameObject.GetComponent<CursorController>();
    edgeAxes = gameObject.GetComponent<EdgeAxes>();
  }

  void FixedUpdate() {
    handleTransformation();
    handleZoom();

    Vector2 cursorPosition = cursor.CursorPosition;
    edgeAxes.updateEdgeAxes(cursorPosition.x, cursorPosition.y, edgeBoundary);

    updatePosition();
    updateRotation();
  }

  void handleTransformation() {
    // Get movement and rotation amount.
    Vector3 movementAmount = this.getAxisAmount(false);
    movementAmount += this.getEdgeScrollAmount();

    Vector3 rotationAmount = this.getAxisAmount(true);

    this.moveRotate(movementAmount, rotationAmount);
  }

  void handleZoom() {
    float zoomAxis = Input.GetAxis("Zoom");

    if (zoomAxis != 0) {
      Vector3 zoomDirection = this.getZoomDirection(zoomAxis);
      Vector3 nextCameraPosition = mainCamera.transform.position + zoomDirection;
      float nextY = nextCameraPosition.y;
      bool isZoomIn = Mathf.Sign(zoomAxis) == 1.0f;

      if (isZoomIn && nextY > 2.0f || !isZoomIn && nextY < 20.0f) {
        mainCamera.transform.position += zoomDirection;
      }
    }
  }

  private void moveRotate(Vector3 movementAmount, Vector3 rotationAmount) {
    bool isMovementZero = movementAmount.Equals(Vector3.zero);
    bool isRotationZero = rotationAmount.Equals(Vector3.zero);
    if (isMovementZero && isRotationZero) { return; }

    this.movement = this.getTransformation(movementAmount, false);
    this.rotation = this.getTransformation(rotationAmount, true);
  }

  private Vector3 getTransformation(Vector3 amount, bool isRotation) {
    Vector3 otherDirection = Vector3.forward;
    float speed = this.MovementSpeed;

    if (isRotation) {
      otherDirection = Vector3.up;
      speed = this.RotationSpeed;
    }

    Vector3 transformation = Vector3.right + otherDirection;
    return Vector3.Scale(transformation, amount) * speed;
  }

  private Vector3 getAxisAmount(bool isRotation) {
    string[] axisNames = this.getAxisNames(isRotation);
    float xAmount = Input.GetAxis(axisNames[0]);
    float zAmount = Input.GetAxis(axisNames[1]);
    float yAmount = 0.0f;

    if (isRotation) {
      yAmount = zAmount;
      zAmount = 0.0f;
    }

    return new Vector3(xAmount, yAmount, zAmount);
  }

  private string[] getAxisNames(bool isRotation) {
    string[] axisNames = { "Horizontal", "Vertical" };

    if (isRotation) {
      axisNames = this.getRotationAxisNames();
    }

    return axisNames;
  }

  private string[] getRotationAxisNames() {
    string tiltAxis = "Tilt";
    string rotationAxis = "Rotation";
    bool isMiddleMouse = Input.GetButton("Middle Mouse");

    if (isMiddleMouse) {
      tiltAxis = "Mouse Rotate Y";
      rotationAxis = "Mouse Rotate X";
    }

    return new string[] { tiltAxis, rotationAxis };
  }

  private Vector3 getEdgeScrollAmount() {
    if (!isEdgeScrollEnabled) {
      return Vector3.zero;
    }

    Vector3 scrollAmount = Vector3.forward * edgeAxes.y;
    scrollAmount += Vector3.right * edgeAxes.x;
    return scrollAmount;
  }

  private Vector3 getZoomDirection(float zoomAxis) {
    Vector3 cameraPosition = mainCamera.transform.position;
    Vector3 desiredPosition = this.getZoomPosition(cameraPosition);
    Vector3 normalizedPosition = Vector3.Normalize(desiredPosition - cameraPosition);
    float zoomAmount = this.getZoomAmount(desiredPosition, cameraPosition, zoomAxis);
    return normalizedPosition * zoomAmount;
  }

  private Vector3 getZoomPosition(Vector3 currentPosition) {
    Vector2 cursorPosition = cursor.CursorPosition;
    RaycastHit hit;
    Ray cameraCursorRay = mainCamera.ScreenPointToRay(cursorPosition);
    bool isHit = Physics.Raycast(cameraCursorRay, out hit);
    return isHit ? hit.point : currentPosition;
  }

  private float getZoomAmount(
      Vector3 desiredPosition, Vector3 currentPosition, float zoomAxis
  ) {
    float desiredDistance = Vector3.Distance(desiredPosition, currentPosition);
    return desiredDistance * zoomAxis * this.ZoomSpeed;
  }

  private void updatePosition() {
    transform.Translate(movement);
    movement = Vector3.zero;
    this.clampPosition();
  }

  private void updateRotation() {
    transform.Rotate(rotation);
    rotation = Vector3.zero;
    this.clampRotation();
  }

  private void clampPosition() {
    Vector3 currentPosition = transform.position;
    currentPosition.x = Mathf.Clamp(currentPosition.x, -10.0f, 10.0f);
    currentPosition.z = Mathf.Clamp(currentPosition.z, -10.0f, 10.0f);
    currentPosition.y = 0.0f;
    transform.position = currentPosition;
  }

  private void clampRotation() {
    Vector3 currentRotation = transform.eulerAngles;
    currentRotation.x = Mathf.Clamp(currentRotation.x, 2.0f, 45.0f);
    currentRotation.z = 0.0f;
    transform.eulerAngles = currentRotation;
  }
}