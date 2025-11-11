using UnityEngine;

public class PidDDAController
{
    // Ganhos do PID
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }

    // Setpoint (valor desejado)
    public float Setpoint { get; set; }

    // Tempo de amostragem
    public float Time { get; set; }

    // Termos internos do PID
    private float _integral;
    private float _previousError;
    private bool _firstCompute = true;

    // Construtor
    public PidDDAController(float kp, float ki, float kd, float setpoint, float time)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
        Setpoint = setpoint;
        Time = time;
        Reset();
    }

    // Método principal: cálculo PID
    public float Compute(float processVariable)
    {
        float error = Setpoint - processVariable;

        // Termo proporcional
        float P = Kp * error;

        // Termo integral
        _integral += error * Time;
        float I = Ki * _integral;

        // Termo derivativo
        float derivative = 0f;
        if (!_firstCompute)
        {
            derivative = (error - _previousError) / Time;
        }
        float D = Kd * derivative;

        // Atualiza estado
        _previousError = error;
        _firstCompute = false;

        // Saída final
        return P + I + D;
    }

    // Método para resetar o controlador
    public void Reset()
    {
        _integral = 0.00f;
        _previousError = 0.00f;
        _firstCompute = true;
    }

}
