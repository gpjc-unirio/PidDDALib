using System.Security.Cryptography;
using UnityEngine;

public class SpeedPIDController : MonoBehaviour
{
    public static PidDDAController PidController;

    public static float setPoint = 5.000f;
    public static float inputVariable;
    public static float outputVariable = 0.00f;

    private void Start()
    {
        float kp = 0.8f;
        float ki = 0.001f;
        float kd = 75f;

        PidController = new PidDDAController(kp, ki, kd, setPoint, Time.deltaTime);
    }

    private void Update()
    {
        if(PidController != null) { 
            PidController.Time = Time.deltaTime;
            outputVariable = PidController.Compute(inputVariable);

            print("-----------------Pidout: " + outputVariable + " ////// SetPoint: " + PidController.Setpoint);
        }
    }
}