using UnityEngine;
namespace DesertQuiet.Player
{
    [RequireComponent(typeof(ThirdPersonMobileController))]
    public sealed class TouchMovePad : MonoBehaviour
    {
        private ThirdPersonMobileController motor;
        private Vector2 input;
        private void Awake() { motor = GetComponent<ThirdPersonMobileController>(); }
        private void OnGUI()
        {
            const float s = 62f;
            float x = 22f, y = Screen.height - 3f * s - 24f;
            GUI.color = new Color(1f, 1f, 1f, 0.78f);
            if (GUI.RepeatButton(new Rect(x + s, y, s, s), "▲")) input.y = 1;
            else if (GUI.RepeatButton(new Rect(x + s, y + 2*s, s, s), "▼")) input.y = -1;
            else input.y = 0;
            if (GUI.RepeatButton(new Rect(x, y + s, s, s), "◀")) input.x = -1;
            else if (GUI.RepeatButton(new Rect(x + 2*s, y + s, s, s), "▶")) input.x = 1;
            else input.x = 0;
            motor.SetMoveInput(input);
        }
        private void OnDisable() { if (motor != null) motor.SetMoveInput(Vector2.zero); }
    }
}