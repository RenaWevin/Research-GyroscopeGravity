
using UnityEngine;
using UnityEngine.UI;

public class GyroscopeGame : MonoBehaviour {

    #region 物件參考區

    [SerializeField] Text Text_Title;
    [SerializeField] Text Text_GravityX;
    [SerializeField] Text Text_NewGravityX;

    [SerializeField] InputField InputField_Range;
    [SerializeField] InputField InputField_Speed;
    [SerializeField] InputField InputField_Acceleration;

    [SerializeField] RectTransform RectTransform_PhoneRotateStatus;

    [SerializeField] Toggle Toggle_EnableSimulator;
    [SerializeField] Slider Slider_Simulator;
    [SerializeField] Text Text_SimulatorValue;
    [SerializeField] Button Button_SetSimulatorToZero;

    [SerializeField] GyroBarObject gyroBarObject_Direct;
    [SerializeField] GyroBarObject gyroBarObject_Velocity;
    [SerializeField] GyroBarObject gyroBarObject_Acceleration;


    #endregion
    #region 數值參考區

    private float gravityX {
        get {
            if (Toggle_EnableSimulator.isOn) {
                return simulatorValue;
            } else {
                return Input.gyro.gravity.x;
            }
        }
    }
    private float newGravityX { get { return range == 0 ? 0 : Mathf.Max(-1f, Mathf.Min(1f, gravityX / range)); } }

    private float simulatorValue { get { return Slider_Simulator.value * 0.01f; } }

    private float value_gyroBarObject_Direct = 0.5f;
    private float value_gyroBarObject_Velocity = 0.5f;
    private float value_gyroBarObject_Acceleration = 0.5f;

    private float range;
    private float speedParam;
    private float accelerationParam;

    private float nowSpeed_gyroBar3 = 0f;

    #endregion

    void Start() {
        Toggle_EnableSimulator.isOn = !SystemInfo.supportsGyroscope;
        if (SystemInfo.supportsGyroscope) {
            Text_Title.text = "陀螺儀 <color=#4D982C>✓</color>";
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.05f;
        }
        Slider_Simulator.onValueChanged.AddListener((_) => { Text_SimulatorValue.text = simulatorValue.ToString("F2"); });
        Button_SetSimulatorToZero.onClick.AddListener(() => { Slider_Simulator.value = 0; });

        InputField_Range.onEndEdit.AddListener(ConvertToRange);
        ConvertToRange(InputField_Range.text);
        InputField_Speed.onEndEdit.AddListener(ConvertToSpeed);
        ConvertToSpeed(InputField_Speed.text);
        InputField_Acceleration.onEndEdit.AddListener(ConvertToAcceleration);
        ConvertToAcceleration(InputField_Acceleration.text);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        Text_GravityX.text = $"<size=32>Gravity X: </size>{gravityX:F4}";
        Text_NewGravityX.text = $"<size=32>New Gravity X: </size>{newGravityX:F4}";

        RectTransform_PhoneRotateStatus.localEulerAngles = new Vector3(0, 0, (-90) * gravityX);

        UpdateDisplay_Pointers();
    }

    void FixedUpdate() {
        //直接反映
        SetValueToRange0to1((newGravityX / 2f) + 0.5f, out value_gyroBarObject_Direct);
        //等速度
        float newValue_Velocity = value_gyroBarObject_Velocity + (newGravityX * speedParam * Time.fixedDeltaTime);
        SetValueToRange0to1(newValue_Velocity, out value_gyroBarObject_Velocity);
        //加速度 (撞牆之後速度不能繼續往牆壁發展)
        nowSpeed_gyroBar3 += (newGravityX * accelerationParam * Time.fixedDeltaTime);
        if (value_gyroBarObject_Acceleration >= 1f - float.Epsilon) {
            nowSpeed_gyroBar3 = Mathf.Min(nowSpeed_gyroBar3, 0f);
        }
        if (value_gyroBarObject_Acceleration <= 0f + float.Epsilon) {
            nowSpeed_gyroBar3 = Mathf.Max(nowSpeed_gyroBar3, 0f);
        }
        float newValue_Acceleration = value_gyroBarObject_Acceleration + (nowSpeed_gyroBar3 * Time.fixedDeltaTime);
        SetValueToRange0to1(newValue_Acceleration, out value_gyroBarObject_Acceleration);
    }

    #region 轉換參數

    private void SetValueToRange0to1(float newValue, out float container) {
        container = Mathf.Max(0f, Mathf.Min(1f, newValue));
    }

    private void ConvertToRange(string value) {
        float.TryParse(value, out float newRange);
        newRange = Mathf.Max(0f, Mathf.Min(1f, newRange));
        InputField_Range.text = newRange.ToString();
        range = newRange;
    }
    private void ConvertToSpeed(string value) {
        float.TryParse(value, out float newSpeed);
        newSpeed = Mathf.Max(0f, Mathf.Min(10f, newSpeed));
        InputField_Speed.text = newSpeed.ToString();
        speedParam = newSpeed;
    }
    private void ConvertToAcceleration(string value) {
        float.TryParse(value, out float newAcceleration);
        newAcceleration = Mathf.Max(0f, Mathf.Min(10f, newAcceleration));
        InputField_Acceleration.text = newAcceleration.ToString();
        accelerationParam = newAcceleration;
    }

    #endregion
    #region 顯示相關

    private void UpdateDisplay_Pointers() {
        gyroBarObject_Direct.SetValue(value_gyroBarObject_Direct);
        gyroBarObject_Velocity.SetValue(value_gyroBarObject_Velocity);
        gyroBarObject_Acceleration.SetValue(value_gyroBarObject_Acceleration);
    }

    #endregion

}
