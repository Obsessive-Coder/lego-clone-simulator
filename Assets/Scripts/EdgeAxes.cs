using UnityEngine;

public class EdgeAxes : MonoBehaviour {
  private float screenWidth;
  private float screenHeight;

  private Vector2 edgeAxes;

  public float x {
    get { return edgeAxes.x; }
  }

  public float y {
    get { return edgeAxes.y; }
  }

  void Start() {
    screenWidth = Screen.width;
    screenHeight = Screen.height;
  }

  public void updateEdgeAxes(float cursorX, float cursorY, float edgeBoundary) {
    Vector2 positiveAxes = this.getAxis(cursorX, cursorY, edgeBoundary, false);
    Vector2 negativeAxes = this.getAxis(cursorX, cursorY, edgeBoundary, true);
    edgeAxes = positiveAxes + negativeAxes;
  }

  private Vector2 getAxis(float cursorX, float cursorY, float edgeBoundary, bool isNegative) {
    float subtrahendX = isNegative ? edgeBoundary : screenWidth - edgeBoundary;
    float subtrahendY = isNegative ? edgeBoundary : screenHeight - edgeBoundary;
    float x = this.clampEdgeAxisValue(cursorX - subtrahendX, isNegative);
    float y = this.clampEdgeAxisValue(cursorY - subtrahendY, isNegative);
    return new Vector2(x, y);
  }

  private float clampEdgeAxisValue(float value, bool isNegative) {
    float min = isNegative ? -1.0f : 0.0f;
    float max = isNegative ? 0.0f : 1.0f;
    return Mathf.Clamp(value, min, max);
  }
}
