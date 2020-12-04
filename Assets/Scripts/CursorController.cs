using UnityEngine;

public class CursorController : MonoBehaviour {
  private float screenWidth;
  private float screenHeight;

  public Texture2D image;
  public int size = 32;

  private Vector2 cursorPosition;
  public Vector2 CursorPosition {
    get { return cursorPosition; }
  }

  public float sensitivity = 100.0f;
  public float Sensitivity {
    get { return sensitivity * Time.deltaTime; }
    set { sensitivity = value; }
  }

  void Start() {
    Cursor.visible = false;
    screenWidth = Screen.width;
    screenHeight = Screen.height;
    cursorPosition = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
  }

  void FixedUpdate() {
    this.move();
    this.clampPosition();
  }

  private void OnGUI() {
    float cursorX = cursorPosition.x;
    float cursorY = screenHeight - cursorPosition.y;
    Rect cursor = new Rect(cursorX, cursorY, size, size);
    GUI.DrawTexture(cursor, image);
  }

  private void move() {
    float axisX = Input.GetAxis("Left Horizontal");
    float axisY = Input.GetAxis("Left Vertical");
    float moveSensitivity = this.Sensitivity;

    if (axisX == 0.0f && axisY == 0.0f) {
      axisX = Input.GetAxis("Mouse X");
      axisY = Input.GetAxis("Mouse Y");
      moveSensitivity *= 20.0f;
    }

    this.cursorPosition += new Vector2(axisX, axisY) * moveSensitivity;
  }

  private void clampPosition() {
    cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0.0f, screenWidth - 10.0f);
    cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0.0f, screenHeight - 10.0f);
  }
}
