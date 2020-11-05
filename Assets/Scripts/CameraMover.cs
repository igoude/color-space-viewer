using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Transform m_center;
    Vector3 m_previousPosition;
    Vector3 m_currentPosition;
    float m_speed = 0.02f;
    
    // Update is called once per frame
    void Update()
    {
        // Compute rotation
        if (Input.GetMouseButtonDown(0)) {
            m_previousPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) {
            m_currentPosition = Input.mousePosition;

            this.transform.RotateAround(m_center.position, Vector3.up, -(m_previousPosition - m_currentPosition).x * m_speed);

            float angle = Vector3.Angle(this.transform.forward, Vector3.up);
            if (angle >= 90.0f && (m_previousPosition-m_currentPosition).y < 0.0f || angle <= 150.0f && (m_previousPosition-m_currentPosition).y > 0.0f) {
                this.transform.RotateAround(m_center.position, this.transform.right, (m_previousPosition-m_currentPosition).y * m_speed);
            }
        }
    }
}
