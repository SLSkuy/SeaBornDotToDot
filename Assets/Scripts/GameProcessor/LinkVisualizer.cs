using System.Collections.Generic;
using Cell;
using UnityEngine;

namespace GameProcessor
{
    [RequireComponent(typeof(LineRenderer))]
    public class LinkVisualizer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        [Header("连线设置")]
        public Color lineColor = Color.yellow;
        public float lineWidth = 0.1f;
        public float displayTime = 0.3f; // 连线显示时长

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 0;
            _lineRenderer.startColor = lineColor;
            _lineRenderer.endColor = lineColor;
            _lineRenderer.startWidth = lineWidth;
            _lineRenderer.endWidth = lineWidth;
        }

        /// <summary>
        /// 绘制匹配连线，按照路径绘制折线
        /// </summary>
        /// <param name="path">单元格路径，从起点到终点</param>
        public void DrawPath(List<DotToDotCell> path)
        {
            if (path == null || path.Count < 2) return;
            
            _lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                _lineRenderer.SetPosition(i, path[i].transform.position);
            }

            // 显示一段时间后自动隐藏
            CancelInvoke(nameof(ClearLine));
            Invoke(nameof(ClearLine), displayTime);
        }

        public void ClearLine()
        {
            _lineRenderer.positionCount = 0;
        }
    }
}