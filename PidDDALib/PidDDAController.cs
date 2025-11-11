using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PidDDALib
{
    public class PidDDAController
    {
        // PID gain factors
        public float Kp { get; set; }
        public float Ki { get; set; }
        public float Kd { get; set; }

        // Setpoint (param value to reach)
        public float Setpoint { get; set; }

        // sample time
        public float Time { get; set; }

        // PID internal terms
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

        // PIC calculate
        public float Compute(float processVariable)
        {
            // erros difference
            float error = Setpoint - processVariable;

            // proportional term
            float P = Kp * error;

            // integrative term
            _integral += error * Time;
            float I = Ki * _integral;

            // derivative term
            float derivative = 0f;
            if (!_firstCompute)
            {
                derivative = (error - _previousError) / Time;
            }
            float D = Kd * derivative;

            // update pid state
            _previousError = error;
            _firstCompute = false;

            // final output
            return P + I + D;
        }

        // resete pid contrller
        public void Reset()
        {
            _integral = 0.00f;
            _previousError = 0.00f;
            _firstCompute = true;
        }
    }
}
