using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


/*
 * 这个代码的作用是：
 * 提供常用的六边形地图相关功能
 * 
 * 其中包含了：
 * 1. 生成六边形地图（非静态，）
 * 2. 寻找一条能连接2个六边形点的最短路径(静态，接受2六边形和地图)
 * 3. 寻找一个形状周围一定距离的区域
 * 
 */

public class MapToolBox : MonoBehaviour
{
    [Header("Map Unit Reference")]
   // public GameObject HexRef;  //这是生成需要的六边形
   // public Vector3 centerPos;  //这是0，0所在的世界坐标
    public HexManager manager;
    public float unitL;
    public int mapWidth;
    public List<Vector2> noHex;
    public List<Vector2> extraHex;
    Dictionary<Vector2, IMapUnit> allMapUnits = new Dictionary<Vector2, IMapUnit>();

    private void Start()
    {
        //GenerateHexMap(mapWidth);
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pivot">这个是作用的中心位置</param>
    /// <param name="range">这个是搜寻的范围</param>
    /// <param name="mapUnits">这个信息是所有的六边形的位置</param>
    /// <returns></returns>
    public static List<Vector2> FindAllPointsForFight(Vector2 pivot, int range, Dictionary<Vector2, IMapUnit> mapUnits)
    {
        List<Vector2> points = new List<Vector2>();
        Queue<(Vector2 point, int steps)> queue = new Queue<(Vector2, int)>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        queue.Enqueue((pivot, 0));
        visited.Add(pivot);
        if (mapUnits.ContainsKey(pivot))
            points.Add(pivot);

        while (queue.Count > 0)
        {
            (Vector2 point, int steps) current = queue.Dequeue();

            foreach (Vector2 dir in Directions)
            {
                Vector2 neighbor = new Vector2(current.point.x + dir.x, current.point.y + dir.y);
                if (!visited.Contains(neighbor) && mapUnits.ContainsKey(neighbor) && current.steps < range)
                {
                    visited.Add(neighbor);
                    queue.Enqueue((neighbor, current.steps + 1));
                    points.Add(neighbor);
                }
            }
        }

        return points;
    }


    //此函数用于生成基础的完整六边形地图
    //public void GenerateHexMap(int n)
    //{
    //    // 定义队列
    //    Queue<Vector2> queue = new Queue<Vector2>();
    //    // 访问标记
    //    HashSet<Vector2> visited = new HashSet<Vector2>();
    //    // 记录pre hex
    //    Dictionary<Vector2, Vector2> allHexPre = new Dictionary<Vector2, Vector2>();

    //    // 初始节点
    //    Vector2 start = new Vector2(0,0);
    //    queue.Enqueue(start);
    //    visited.Add(start);

    //    //这里的逻辑是，使用BFS去探索所有节点，找到范围之内的。每访问一个就生成在对应的位置。
    //    //每个六边形就是一个节点，和自己周围六个节点相连
    //    while (queue.Count > 0)
    //    {
    //        var current = queue.Dequeue();
    //        //Console.WriteLine($"Visiting node: ({current.x}, {current.y})");
    //        GameObject tempObj;
    //        if (current == new Vector2(0,0))
    //            tempObj = Instantiate(HexRef, centerPos, Quaternion.identity);
    //        else
    //        {
    //            //如果不是初始节点，就根据探索到自己的节点去生成
    //            float rAngle = AngleFromDirection(- allHexPre[current] +current);
    //            Vector3 rPos = new Vector3(unitL * Mathf.Cos(rAngle), unitL*Mathf.Sin(rAngle), 0);

    //            tempObj = Instantiate(HexRef,
    //                allMapUnits[allHexPre[current]].returnGmo().transform.position + rPos,
    //                Quaternion.identity);
    //        }
    //        //记录信息
    //        allMapUnits.Add(current, tempObj.GetComponent<IMapUnit>());
    //        tempObj.GetComponent<IMapUnit>().changePos(current);
    //        tempObj.GetComponent<IMapUnit>().changeManager(manager);

    //        //根据当前访问的节点，探索周围六个节点
    //        foreach (var direction in Directions)
    //        {
    //            var neighbor = new Vector2(current.x + direction.x, current.y + direction.y);
    //            //Debug.Log(neighbor);
    //            if (Math.Abs(neighbor.x) + Math.Abs(neighbor.y) + Math.Abs(neighbor.x + neighbor.y) <= n * 2 && !visited.Contains(neighbor))
    //            {
    //                if (!noHex.Contains(neighbor))
    //                {
    //                    queue.Enqueue(neighbor);
    //                    visited.Add(neighbor);
    //                    allHexPre.Add(neighbor, current);
    //                }

    //            }
    //        }
    //    }

    //    foreach (Vector2 v in extraHex)
    //    {
    //        if (!allMapUnits.ContainsKey(v))
    //        {
    //            foreach (Vector2 dir in Directions)
    //            {
    //                if (allMapUnits.ContainsKey(v + dir))
    //                {
    //                    GameObject tempObj;
    //                    //如果不是初始节点，就根据探索到自己的节点去生成
    //                    float rAngle = AngleFromDirection(-dir);
    //                    Vector3 rPos = new Vector3(unitL * Mathf.Cos(rAngle), unitL * Mathf.Sin(rAngle), 0);

    //                    tempObj = Instantiate(HexRef,
    //                        allMapUnits[v+dir].returnGmo().transform.position + rPos,
    //                        Quaternion.identity);
    //                    allMapUnits.Add(v, tempObj.GetComponent<IMapUnit>());
    //                    tempObj.GetComponent<IMapUnit>().changePos(v);
    //                    tempObj.GetComponent<IMapUnit>().changeManager(manager);

    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    manager.allMapUnits = allMapUnits;

    //}


    //这个依然是使用BFS的理念，使用就是传入两个map unit 和map
    public static List<Vector2> FindPath(IMapUnit startM, IMapUnit endM, Dictionary<Vector2, IMapUnit> map)
    {
        Vector2 start = startM.returnPos();
        Vector2 end = endM.returnPos();
        if (!map.ContainsKey(start) || !map.ContainsKey(end))
            throw new ArgumentException("Start or end position is not in the map.");

        Queue<Vector2> queue = new Queue<Vector2>();
        Dictionary<Vector2, Vector2> predecessors = new Dictionary<Vector2, Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        queue.Enqueue(start);
        visited.Add(start);
        predecessors[start] = start; // Start has no predecessor, points to itself

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
            {
                return ReconstructPath(predecessors, start, end);
            }

            foreach (var direction in Directions)
            {
                var neighbor = current + direction;
                if (map.ContainsKey(neighbor) && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    predecessors[neighbor] = current;
                }
            }
        }

        return new List<Vector2>(); // No path found
    }

    

    //pivot代表选择中心，n代表范围/距离，shapeX就是选择的形状，mapUnits就是地图
    public static List<Vector2> FindAllPointsWithinDistance(Vector2 pivot, int n, List<Vector2> shapeX, Dictionary<Vector2, IMapUnit> mapUnits)
    {
        //这个函数用于基于一个点pivot,把这个pivot当做shape的中心点，或者说0,0点
        //然后它先找到了所有在范围里的0，0点，也就是距离pivot距离在n以内的点
        //对着这些点开始复现本来的shape。对于任意一个点，只要shape里有一个位置不在图里，就不会添加。

        HashSet<Vector2> tempTable = new HashSet<Vector2>(); // 临时表，存储距离pivot n以内的点
        HashSet<Vector2> finalPoints = new HashSet<Vector2>(); // 最终返回的点集
        Queue<(Vector2 point, int steps)> queue = new Queue<(Vector2, int)>();

        bool allPExist = true;
        foreach (Vector2 ad in shapeX)
        {
            if (!mapUnits.ContainsKey(pivot + ad)) allPExist = false;
        }
        if (!allPExist) return new List<Vector2>();

        // 从pivot开始进行BFS，找出n距离内所有点
        queue.Enqueue((pivot, 0));
        while (queue.Count > 0)
        {
            (Vector2 current, int currentSteps) = queue.Dequeue();
            tempTable.Add(current);

            if (currentSteps < n)
            {
                foreach (Vector2 dir in Directions)
                {
                    Vector2 neighbor = current + dir;
                    if (!tempTable.Contains(neighbor) && mapUnits.ContainsKey(neighbor))
                    {
                        queue.Enqueue((neighbor, currentSteps + 1));
                    }
                }
            }
        }

        // 检查临时表中的每个点与shapeX的组合
        foreach (Vector2 a in tempTable)
        {
            bool allPointsExist = true;
            List<Vector2> candidatePoints = new List<Vector2>();

            foreach (Vector2 shapePoint in shapeX)
            {
                Vector2 combinedPoint = a + shapePoint;
                if (mapUnits.ContainsKey(combinedPoint))
                {
                    candidatePoints.Add(combinedPoint);
                }
                else
                {
                    allPointsExist = false;
                    break;
                }
            }

            if (allPointsExist)
            {
                foreach (Vector2 point in candidatePoints)
                {
                    finalPoints.Add(point);
                }
            }
        }

        return new List<Vector2>(finalPoints);
    }

    public static List<Vector2> FindAllPointsWithinDistanceForRM(Vector2 pivot, int n, List<Vector2> shapeX, Dictionary<Vector2, IMapUnit> mapUnits)
    {
        //这个函数用于基于一个点pivot,把这个pivot当做shape的中心点，或者说0,0点
        //然后它先找到了所有在范围里的0，0点，也就是距离pivot距离在n以内的点
        //对着这些点开始复现本来的shape。对于任意一个点，只要shape里有一个位置不在图里，就不会添加。

        HashSet<Vector2> tempTable = new HashSet<Vector2>(); // 临时表，存储距离pivot n以内的点
        HashSet<Vector2> finalPoints = new HashSet<Vector2>(); // 最终返回的点集
        Queue<(Vector2 point, int steps)> queue = new Queue<(Vector2, int)>();

        // 从pivot开始进行BFS，找出n距离内所有点
        queue.Enqueue((pivot, 0));
        while (queue.Count > 0)
        {
            (Vector2 current, int currentSteps) = queue.Dequeue();
            tempTable.Add(current);

            if (currentSteps < n)
            {
                foreach (Vector2 dir in Directions)
                {
                    Vector2 neighbor = current + dir;
                    if (!tempTable.Contains(neighbor) && mapUnits.ContainsKey(neighbor) && mapUnits[neighbor].returnMode() == SelectMode.rangeMode)
                    {
                        queue.Enqueue((neighbor, currentSteps + 1));
                    }
                }
            }
        }

        // 检查临时表中的每个点与shapeX的组合
        foreach (Vector2 a in tempTable)
        {
            bool allPointsExist = true;
            List<Vector2> candidatePoints = new List<Vector2>();

            foreach (Vector2 shapePoint in shapeX)
            {
                Vector2 combinedPoint = a + shapePoint;
                if (mapUnits.ContainsKey(combinedPoint) && mapUnits[combinedPoint].returnMode() == SelectMode.rangeMode)
                {
                    candidatePoints.Add(combinedPoint);
                }
                else
                {
                    allPointsExist = false;
                    break;
                }
            }

            if (allPointsExist)
            {
                foreach (Vector2 point in candidatePoints)
                {
                    finalPoints.Add(point);
                }
            }
        }

        return new List<Vector2>(finalPoints);
    }

    public static List<Vector2> FindAllPointsWithinDistanceForRMAndNoEn(Vector2 pivot, int n, List<Vector2> shapeX, Dictionary<Vector2, IMapUnit> mapUnits)
    {
        //这个函数用于基于一个点pivot,把这个pivot当做shape的中心点，或者说0,0点
        //然后它先找到了所有在范围里的0，0点，也就是距离pivot距离在n以内的点
        //对着这些点开始复现本来的shape。对于任意一个点，只要shape里有一个位置不在图里，就不会添加。

        HashSet<Vector2> tempTable = new HashSet<Vector2>(); // 临时表，存储距离pivot n以内的点
        HashSet<Vector2> finalPoints = new HashSet<Vector2>(); // 最终返回的点集
        Queue<(Vector2 point, int steps)> queue = new Queue<(Vector2, int)>();

        // 从pivot开始进行BFS，找出n距离内所有点
        queue.Enqueue((pivot, 0));
        Debug.Log("here now");
        while (queue.Count > 0)
        {
            (Vector2 current, int currentSteps) = queue.Dequeue();
            tempTable.Add(current);

            if (currentSteps < n)
            {
                foreach (Vector2 dir in Directions)
                {
                    Vector2 neighbor = current + dir;
                    if (!tempTable.Contains(neighbor) && mapUnits.ContainsKey(neighbor) && mapUnits[neighbor].returnMode() == SelectMode.rangeMode && mapUnits[neighbor].returnTileType()!= UnitType.Enemy_Unit)
                    {
                        queue.Enqueue((neighbor, currentSteps + 1));
                        Debug.Log(neighbor);
                    }
                }
            }
        }

        // 检查临时表中的每个点与shapeX的组合
        foreach (Vector2 a in tempTable)
        {
            bool allPointsExist = true;
            List<Vector2> candidatePoints = new List<Vector2>();

            foreach (Vector2 shapePoint in shapeX)
            {
                Vector2 combinedPoint = a + shapePoint;
                if (mapUnits.ContainsKey(combinedPoint) && mapUnits[combinedPoint].returnMode() == SelectMode.rangeMode && mapUnits[combinedPoint].returnTileType() != UnitType.Enemy_Unit)
                {
                    candidatePoints.Add(combinedPoint);
                }
                else
                {
                    allPointsExist = false;
                    break;
                }
            }

            if (allPointsExist)
            {
                foreach (Vector2 point in candidatePoints)
                {
                    finalPoints.Add(point);
                }
            }
        }

        return new List<Vector2>(finalPoints);
    }

    /*
     * 一些static的通用方法。
     */

    public static readonly Vector2[] Directions = new[]
    {
        new Vector2(1,0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1)
    };

    public static float AngleFromDirection(Vector2 direction)
    {
        int index = Array.IndexOf(Directions, direction);
        if (index == -1) return -1; // Direction not found
        return index * 60*Mathf.Deg2Rad; // Assuming a pointy-top hex layout
    }

    private static List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> predecessors, Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 step = end;

        while (step != start)
        {
            path.Add(step);
            step = predecessors[step];
        }
        path.Add(start);

        path.Reverse();
        return path;
    }
}
