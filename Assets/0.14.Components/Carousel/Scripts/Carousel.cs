using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/*
* 作者：张骥
* 时间：2021-03-03
* 作用：轮播图
*/

/// <summary>
/// 移动方向
/// </summary>
public enum MoveDirection
{
    LeftToRight = -1,
    RightToLeft = 1,
    UPToDown = 2,
    DownToUp = -2
}

public class Carousel : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region 公共变量
    [Header("生成时所需物体")]
    public RollCell prefab;
    [Header("是否需要正中元素重新刷新数据")]
    public bool isNeedUpdate = false;
    [Header("物体之间间隔")]
    public float Spacing;
    [Header("自动轮播")]
    public bool AutoLoop = true;
    [Header("拖动")]
    public bool Drag = true;
    [Header("移动方向")]
    public MoveDirection moveDirection = MoveDirection.RightToLeft;
    [Header("过图速度（帧）")]
    public int tweenStepCount = 50;
    [Header("停图时间（秒）")]
    public float LoopSpace = 3;
    // 当前处于正中的元素
    public int CurrentIndex { get { return m_index; } }
    #endregion

    #region 私有变量
    //移动结束回调
    private Action<int> MoveEndCallback;
    //父物体大小
    private RectTransform viewRectTran;
    //当前是否拖动
    private bool m_Dragging = false;
    //当前物体状态是否移动完成
    private bool m_IsNormalizing = false;
    //是否在移动
    private bool isMoving = true;
    //当前处于正中物体位置
    private Vector2 m_CurrentPos;
    //当前处于正中元素移动进度——与tweenStepCount对应
    private int m_currentStep = 0;
    //拖动目标位置
    private Vector2 m_PrePos;
    //当前处于正中的元素
    private int m_index = 0;
    //子物体大小
    private RectTransform cellRect;
    //是否子物体排列长度大于父物体
    private bool contentCheckCache = true;
    //生成物体
    private List<RollCell> cellList = new List<RollCell>();
    private List<RollCell> cellListPool = new List<RollCell>();
    //线程
    private Coroutine moveCoroutine;
    /// <summary>
    /// 轮播方向--1为向左移动，-1为向右移动，2为向下移动，-2为向上移动
    /// </summary>
    private int LoopDir { get { return (int)moveDirection; } }
    //中央元素停止时间——与LoopSpace对应
    private float currTimeDelta = 0;
    //父物体最左端
    private float viewRectXMin
    {
        get
        {
            Vector3[] v = new Vector3[4];
            viewRectTran.GetWorldCorners(v);
            return v[0].x;
        }
    }
    //父物体最右端
    private float viewRectXMax
    {
        get
        {
            Vector3[] v = new Vector3[4];
            viewRectTran.GetWorldCorners(v);
            return v[3].x;
        }
    }
    //父物体最下端
    private float viewRectYMin
    {
        get
        {
            Vector3[] v = new Vector3[4];
            viewRectTran.GetWorldCorners(v);
            return v[0].y;
        }
    }
    //父物体最上端
    private float viewRectYMax
    {
        get
        {
            Vector3[] v = new Vector3[4];
            viewRectTran.GetWorldCorners(v);
            return v[2].y;
        }
    }
    //缓存数据
    object[] cacheList;
    #endregion

    #region 接口
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="MoveEndCallback"></param>
    public void InitBanner(Action<int> MoveEndCallback = null)
    {
        viewRectTran = GetComponent<RectTransform>();
        this.MoveEndCallback = MoveEndCallback;
    }

    /// <summary>
    /// 根据传入数据生成、刷新预制体并开始移动
    /// </summary>
    /// <param name="dataList"></param>
    /// <typeparam name="T"></typeparam>
    public void StartMove<T>(List<T> dataList)
    {
        EndMove();
        UpdateData(dataList);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(DoMove());
    }

    /// <summary>
    /// 停止移动并关闭所有预制体
    /// </summary>
    public void EndMove()
    {
        isMoving = true;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        DestoryAllItem();
    }

    /// <summary>
    /// 移动到指定元素
    /// </summary>
    /// <param name="index"></param>
    public void SetItemIndex(int index)
    {
        MoveToIndex(index);
    }

    #endregion


    #region 内部逻辑
    #region 生成物体
    /// <summary>
    /// 根据传入数据列表生成预制体并刷新布局排列
    /// </summary>
    /// <param name="dataList"></param>
    /// <typeparam name="T"></typeparam>
    private void UpdateData<T>(List<T> dataList)
    {
        //Debug.Log(dataList.Count);
        //缓存数据获取
        if (dataList != null && dataList.Count != 0)
        {
            cacheList = new object[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                cacheList[i] = dataList[i];
            }
        }
        else
        {
            cacheList = null;
        }

        //刷新数据
        if (this.prefab != null)
        {
            if (cacheList != null)
            {
                for (int i = 0; i < cacheList.Length; i++)
                {
                    CreateItem(i, cacheList[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                RollCell cell = viewRectTran.GetChild(i).GetComponent<RollCell>();
                cell.InitItem();
                cell.ShowItem(i, cacheList != null ? cacheList[i] : null);
                cellList.Add(cell);
            }
        }
        resizeChildren();
    }

    /// <summary>
    /// 生成单个预制体
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    private RollCell CreateItem(int index, object data)
    {
        RollCell cell = null;
        if (cellListPool.Count > 0)
        {
            cell = cellListPool[cellListPool.Count - 1];
            cellListPool.RemoveAt(cellListPool.Count - 1);
        }
        else
        {
            GameObject newone = Instantiate(this.prefab.gameObject, viewRectTran);
            newone.transform.localPosition = Vector3.zero;
            newone.transform.localPosition = Vector3.one;
            cell = newone.GetComponent<RollCell>();
            cell.InitItem();
        }
        cell.transform.SetSiblingIndex(index);
        cell.ShowItem(index, data);
        cellList.Add(cell);
        return cell;
    }

    /// <summary>
    /// 清空所有
    /// </summary>
    private void DestoryAllItem()
    {
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].DisShowItem();
        }
        cellListPool.AddRange(cellList);
        cellList.Clear();
    }
    #endregion

    #region 布局
    private void resizeChildren()
    {
        cellRect = GetChild(viewRectTran, 0);
        //init child size and pos
        Vector2 delta;
        if (Mathf.Abs(LoopDir) % 2 == 1)
        {
            delta = new Vector2(cellRect.sizeDelta.x + Spacing, 0);
        }
        else
        {
            delta = new Vector2(0, cellRect.sizeDelta.y + Spacing);
        }

        for (int i = 0; i < cellList.Count; i++)
        {
            var t = GetChild(viewRectTran, i);
            if (t)
            {
                t.localPosition = delta * i;
                t.sizeDelta = cellRect.sizeDelta;
            }
        }
        m_IsNormalizing = false;
        m_CurrentPos = Vector2.zero;
        m_currentStep = 0;
    }
    #endregion

    #region 移动
    /// <summary>
    /// 移动（线程）
    /// </summary>
    /// <param name="dataList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerator DoMove()
    {
        do
        {
            yield return new WaitUntil(() => ContentIsLongerThanRect());
            MoveFunc();
        } while (true);
    }

    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="dataList"></param>
    /// <typeparam name="T"></typeparam>
    private void MoveFunc()
    {
        //实现在必要时loop子元素
        if (Application.isPlaying)
        {
            int s = GetBoundaryState();
            LoopCell(s);
        }

        //缓动回指定位置
        if (m_IsNormalizing && EnsureListCanAdjust())
        {
            if (m_currentStep == tweenStepCount)
            {
                m_IsNormalizing = false;
                m_currentStep = 0;
                m_CurrentPos = Vector2.zero;
                return;
            }
            Vector2 delta = m_CurrentPos / tweenStepCount;
            m_currentStep++;
            TweenToCorrect(-delta);
        }

        //自动loop
        if (AutoLoop && !m_IsNormalizing && EnsureListCanAdjust() && isMoving)
        {
            currTimeDelta += Time.deltaTime;
            if (currTimeDelta > LoopSpace)
            {
                currTimeDelta = 0;
                int targetIndex = LoopDir > 0 ? m_index + 1 : m_index - 1;
                MoveToIndex(targetIndex);
            }
        }
        //m_index = Mathf.Abs(LoopDir) % 2 == 1 ? (int)(cellRect.localPosition.x / (cellRect.sizeDelta.x + Spacing - 1)) : (int)(cellRect.localPosition.y / (cellRect.sizeDelta.y + Spacing - 1));
        //m_index = m_index <= 0 ? m_index = Mathf.Abs(m_index) : cellList.Count - m_index;
    }

    /// <summary>
    /// 移动到指定索引
    /// </summary>
    /// <param name="ind"></param>
    private void MoveToIndex(int ind)
    {
        //Debug.Log(ind + ";;" + m_index);
        if (m_IsNormalizing || ind == m_index)
        {
            return;
        }

        int dex = ind >= 0 ? ind : cellList.Count + ind;
        if (dex == cellList.Count)
        {
            dex = 0;
        }

        //是否需要移到正中时刷新数据
        if (isNeedUpdate)
        {
            object data = cacheList != null ? cacheList[dex] : (object)null;
            cellList[dex].UpdateData(dex, data);
        }

        MoveEndCallback?.Invoke(dex);
        this.m_IsNormalizing = true;
        Vector2 offset = Mathf.Abs(LoopDir) % 2 == 1 ? new Vector2(cellRect.sizeDelta.x + Spacing, 0) : new Vector2(0, cellRect.sizeDelta.y + Spacing);
        m_CurrentPos = CalcCorrectDeltaPos() + offset * (ind - m_index);
        m_currentStep = 0;

        m_index = dex;
    }
    #endregion

    /// <summary>
    /// 拖动时刷新子物体布局
    /// </summary>
    /// <param name="position"></param>
    private void SetContentPosition(Vector2 position)
    {
        foreach (RectTransform i in viewRectTran)
        {
            i.localPosition += (Vector3)position;
        }
        return;
    }

    /// <summary>
    /// List是否处于可自由调整状态
    /// </summary>
    /// <returns></returns>
    private bool EnsureListCanAdjust()
    {
        return !m_Dragging && ContentIsLongerThanRect();
    }

    /// <summary>
    /// 内容是否比显示范围大
    /// </summary>
    /// <returns></returns>
    private bool ContentIsLongerThanRect()
    {
        float contentLen;
        float rectLen;
        if (Mathf.Abs(LoopDir) % 2 == 1)
        {
            contentLen = cellList.Count * (cellRect.sizeDelta.x + Spacing) - Spacing;
            rectLen = viewRectTran.rect.xMax - viewRectTran.rect.xMin;

        }
        else
        {
            contentLen = cellList.Count * (cellRect.sizeDelta.y + Spacing) - Spacing;
            rectLen = viewRectTran.rect.yMax - viewRectTran.rect.yMin;
        }
        contentCheckCache = contentLen > rectLen;
        return contentCheckCache;
    }

    /// <summary>
    /// 检测边界情况，分为0未触界，-1左(下)触界，1右(上)触界
    /// </summary>
    /// <returns></returns>
    private int GetBoundaryState()
    {
        RectTransform left;
        RectTransform right;
        left = GetChild(viewRectTran, 0);
        right = GetChild(viewRectTran, cellList.Count - 1);
        Vector3[] l = new Vector3[4];
        left.GetWorldCorners(l);
        Vector3[] r = new Vector3[4];
        right.GetWorldCorners(r);
        if (Mathf.Abs(LoopDir) % 2 == 1)
        {
            if (l[0].x >= viewRectXMin)
            {
                return -1;
            }
            else if (r[3].x < viewRectXMax)
            {
                return 1;
            }
        }
        else
        {
            if (l[0].y >= viewRectYMin)
            {
                return -1;
            }
            else if (r[1].y < viewRectYMax)
            {
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// Loop列表，分为-1把最右(上)边一个移到最左(下)边，1把最左(下)边一个移到最右(上)边
    /// </summary>
    /// <param name="dir"></param>
    private void LoopCell(int dir)
    {
        if (dir == 0)
        {
            return;
        }
        RectTransform MoveCell;
        RectTransform Tarborder;
        Vector2 TarPos;
        if (dir > 0)
        {
            MoveCell = GetChild(viewRectTran, 0);
            Tarborder = GetChild(viewRectTran, cellList.Count - 1);
            MoveCell.SetSiblingIndex(cellList.Count - 1);
        }
        else
        {
            Tarborder = GetChild(viewRectTran, 0);
            MoveCell = GetChild(viewRectTran, cellList.Count - 1);
            MoveCell.SetSiblingIndex(0);
        }
        if (Mathf.Abs(LoopDir) % 2 == 1)
        {

            TarPos = Tarborder.localPosition + new Vector3((cellRect.sizeDelta.x + Spacing) * dir, 0, 0);
        }
        else
        {
            TarPos = Tarborder.localPosition + new Vector3(0, (cellRect.sizeDelta.y + Spacing) * dir, 0);
        }

        MoveCell.localPosition = TarPos;
    }

    /// <summary>
    /// 计算一个最近的正确位置
    /// </summary>
    /// <returns></returns>
    private Vector2 CalcCorrectDeltaPos()
    {
        Vector2 delta = Vector2.zero;
        float distance = float.MaxValue;
        foreach (RectTransform i in viewRectTran)
        {
            var td = Mathf.Abs(i.localPosition.x) + Mathf.Abs(i.localPosition.y);
            if (td <= distance)
            {
                distance = td;
                delta = i.localPosition;
            }
            else
            {
                break;
            }
        }
        return delta;
    }
    /// <summary>
    /// 移动指定增量
    /// </summary>
    private void TweenToCorrect(Vector2 delta)
    {
        foreach (RectTransform i in viewRectTran)
        {
            i.localPosition += (Vector3)delta;
        }
    }

    /// <summary>
    /// 获取单个子物体
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private RectTransform GetChild(RectTransform parent, int index)
    {
        if (parent == null || index >= parent.childCount)
        {
            return null;
        }
        return parent.GetChild(index) as RectTransform;
    }

    #region OverrideFunc&Interface   
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!Drag || !contentCheckCache)
        {
            return;
        }
        Vector2 vector;
        if (((eventData.button == PointerEventData.InputButton.Left) && this.IsActive()) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRectTran, eventData.position, eventData.pressEventCamera, out vector))
        {
            this.m_Dragging = true;
            m_PrePos = vector;
            isMoving = false;
        }
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!Drag)
        {
            return;
        }
        return;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!Drag || !contentCheckCache)
        {
            return;
        }
        Vector2 vector;
        if (((eventData.button == PointerEventData.InputButton.Left) && this.IsActive()) && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRectTran, eventData.position, eventData.pressEventCamera, out vector))
        {
            m_IsNormalizing = false;
            m_CurrentPos = Vector2.zero;
            m_currentStep = 0;
            Vector2 vector2 = vector - this.m_PrePos;
            Vector2 vec = new Vector2(vector2.x, 0);
            this.SetContentPosition(vec);
            m_PrePos = vector;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!Drag || !contentCheckCache)
        {
            return;
        }
        this.m_Dragging = false;
        this.m_IsNormalizing = true;
        m_CurrentPos = CalcCorrectDeltaPos();
        m_currentStep = 0;
        isMoving = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMoving = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMoving = true;
    }
    #endregion
    #endregion
}