using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MapQuadTree
{
    public QuadTreeNode TreeRoot { get { return root; } }
    private QuadTreeNode root;

    private int width;
    private int height;
    private BitArray obstacleBitMap;
    private HashSet<QuadTreeNode> activeNodes = new HashSet<QuadTreeNode>();
    public void SetMapSize(int width, int height) { this.width = width; this.height = height; }
    public void SetObstacleBitMap(BitArray obstacleBitMap) => this.obstacleBitMap = obstacleBitMap;
    public void ClearTree() 
    {
        root = null;
        obstacleBitMap = null;
        activeNodes.Clear();
    }
    public QuadTreeNode BuildQuadTree(int startX, int startY, int size, int minLeafSize)
    {
        QuadTreeNode node = new QuadTreeNode(startX, startY, size);

        if (size <= minLeafSize)
        {
            for (int x = startX; x < startX + size && x < width; x++)
            {
                for (int y = startY; y < startY + size && y < height; y++)
                {
                    if (obstacleBitMap[y * width + x])
                    {
                        MapObstacleObj obj = MapManager.Instance.GetObstacleAt(x, y);
                        if (obj != null) node.obstacles.Add(obj);
                    }
                }
            }

            return node;
        }

        int half = Mathf.CeilToInt(size / 2f);
        node.children = new QuadTreeNode[4];
        node.children[0] = BuildQuadTree(startX, startY, half, minLeafSize);     
        node.children[1] = BuildQuadTree(startX + half, startY, half, minLeafSize);       
        node.children[2] = BuildQuadTree(startX, startY + half, half, minLeafSize);      
        node.children[3] = BuildQuadTree(startX + half, startY + half, half, minLeafSize);

        root = node;
        return node;
    }
    public void QueryNodes(QuadTreeNode node, RectInt range, List<QuadTreeNode> result)
    {
        bool isIntersects = range.Intersects(node.startX, node.startY, node.size);
        if (!isIntersects) return;

        if (node.isLeaf)
        {
            result.Add(node);
            return;
        }

        foreach (var child in node.children)
            QueryNodes(child, range, result);
    }
    

    public void UpdateVisibleNodes(RectInt viewRect)
    {
        List<QuadTreeNode> currentNodes = new();
        QueryNodes(TreeRoot, viewRect, currentNodes);

        HashSet<QuadTreeNode> newSet = new HashSet<QuadTreeNode>(currentNodes);

        foreach (var node in newSet)
        {
            if (!activeNodes.Contains(node))
            {
                foreach (var obj in node.obstacles) obj.SetVisible(true);
            }
        }

        // ��Ĭ�뿪�Ľڵ�
        foreach (var node in activeNodes)
        {
            if (!newSet.Contains(node))
            {
                foreach (var obj in node.obstacles) obj.SetVisible(false);
            }
        }

        activeNodes = newSet;
    }
}
public class QuadTreeNode
{
    public int startX, startY;      
    public int size;             
    public QuadTreeNode[] children; 
    public List<MapObstacleObj> obstacles; 

    public bool isLeaf => children == null;

    public QuadTreeNode(int startX, int startY, int size)
    {
        this.startX = startX;
        this.startY = startY;
        this.size = size;
        this.children = null;
        this.obstacles = new List<MapObstacleObj>();
    }
}
public struct RectInt
{
    public int xMin, yMin, xMax, yMax;

    public RectInt(int xMin, int yMin, int xMax, int yMax)
    {
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;
    }

    public bool Intersects(int sx, int sy, int size)
    {
        int ex = sx + size;
        int ey = sy + size;

        return IsAABBOverlap(xMin, yMin, xMax, yMax, sx,sy,ex,ey);
    }
    private bool IsAABBOverlap(
        int x1Min, int y1Min, int x1Max, int y1Max,
        int x2Min, int y2Min, int x2Max, int y2Max)
    {
        bool overlapX = x1Min < x2Max && x1Max > x2Min;
        bool overlapY = y1Min < y2Max && y1Max > y2Min;
        return overlapX && overlapY;
    }
}
