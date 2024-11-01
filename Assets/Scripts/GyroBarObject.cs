
using UnityEngine;
using UnityEngine.UI;

public class GyroBarObject : MonoBehaviour {

    [SerializeField] private RectTransform Rect_Image_Bar;
    [SerializeField] private RectTransform Rect_Image_Pointer;
    [SerializeField] private Text Text_Value;

    public float value { get; private set; }

    /// <summary>
    /// value範圍為0.0~1.0
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(float value) {
        value = Mathf.Max(0f, Mathf.Min(1f, value));
        this.value = value;

        Text_Value.text = value.ToString("F2");

        float totalWidth = Rect_Image_Bar.rect.width;
        Vector3 pos_Minimum = new Vector3(Rect_Image_Bar.localPosition.x - (totalWidth/2), Rect_Image_Pointer.localPosition.y, Rect_Image_Pointer.localPosition.z);
        Vector3 pos_new = pos_Minimum + (Vector3.right * totalWidth * value);
        Rect_Image_Pointer.localPosition = pos_new;
    }

}
