using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    public Point[] m_waypoints;
    [SerializeField]
    Color m_waypointColor = Color.yellow;
    // Start is called before the first frame update

    void OnDrawGizmos()
    {
        m_waypoints = GetComponentsInChildren<Point>();
        if (m_waypoints == null || m_waypoints.Length <= 1) return;
        for (int i = 0; i < m_waypoints.Length - 1; i++)
        {
            m_waypoints[i].Color = m_waypointColor;
            Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
        }
        m_waypoints[m_waypoints.Length - 1].Color = m_waypointColor;
    }
}
