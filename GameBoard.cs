using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public StateManager State;

    public Transform Select;
    public Transform Highlight;
    public LineRenderer line;
    public bool buildingHighlighted = false;
    public bool HighlightedEnemy = false;
    public Color[] HighCol;
    public Gradient[] ArrowCol;
    public float highTime = 0.1f;
    public float highTimer = 0f;

    public RectTransform[] Playspace;
    public Building buildingPrefab;
    public Dictionary<Building, string> AllBuildings = new Dictionary<Building, string>();
    public List<Building>[] buildings;
    private Grid grid;

    public Vector3 offset = new Vector3(-0.5f, 0.5f);
    private Vector3 mouse;
    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();

        line = Select.GetComponent<LineRenderer>();
        Select.gameObject.SetActive(false);
        Highlight.gameObject.SetActive(false);
        GenerateBoard();
    }
    public void Selected(Transform select)
    {
        Select.position = select.Find("Back").position;
        Select.localScale = select.Find("Back").lossyScale * 1.1f;
        Select.gameObject.SetActive(true);
    }
    void GenerateBoard()
    {
        Building building;
        float i = 0;
        float boardsize = 0;
        RectTransform space;
        buildings = new List<Building>[Playspace.Length];
        while (i < buildings.Length)
        {
            buildings[(int)i] = new List<Building>();
            i++;
        }
        //playspaces are in same order as players. IE Playspace[0] is player[0]'s stuff. Keep in mind when assigning UI stuff later.
        for(int p = 0; p< Playspace.Length;p++)
        {
            i = 0;
            space = Playspace[p];
            Vector3 rec = new Vector3(space.rect.width, space.rect.height, 0);
            Vector3 recPos = space.transform.position;
            recPos.z = 10;
            recPos = Camera.main.ScreenToWorldPoint(recPos);
            rec /= 100;
            rec.x /= grid.cellSize.x + grid.cellGap.x;
            rec.y /= grid.cellSize.y + grid.cellGap.y;
            rec.x = Mathf.Round(rec.x);
            rec.y = Mathf.Round(rec.y);
            boardsize = rec.x * rec.y;
            //print(rec.y/ (grid.cellSize.y + grid.cellGap.y));
            Vector3 pos = Vector3.zero;
            while (i < boardsize)
            {
                building = Instantiate(buildingPrefab, transform);
                
                //get tile based positions
                pos.x = (i % rec.x);
                pos.y = (i - pos.x)/rec.x;
                //convert to world positions
                pos.x *= grid.cellSize.x + grid.cellGap.x;
                pos.y *= grid.cellSize.y + grid.cellGap.y;
                pos += recPos;
                pos = grid.CellToWorld( grid.WorldToCell(pos));
                building.transform.position = pos;
                building.state = State;
                buildings[p].Add(building);
                // so that later you can pass the building and get back string Player/index. String.split("/") will give [0]player[1]index for buildings[][].
                AllBuildings.Add(building, p.ToString() + "/" + (buildings[p].Count - 1).ToString());
                i++;
            }
            GetFreeSpace(p).Cards.Clear();
            GetFreeSpace(p).Cards.Add(0);
            GetFreeSpace(p).player = p;
            GetFreeSpace(p).gameObject.SetActive(true);
        }
    }
    public Building GetFreeSpace(int player)
    {
        foreach (Building bld in buildings[player])
        {
            if (!bld.gameObject.activeInHierarchy)
            {
                return bld;
            }

        }
        // No free spaces
            return null;
    }

    public void BoardActive()
    {
        if (HighlightedEnemy)
        {
            Highlight.GetComponent<Renderer>().material.color = HighCol[1];
            line.colorGradient = ArrowCol[1];
        }
        else
        {
            Highlight.GetComponent<Renderer>().material.color = HighCol[0];
            line.colorGradient = ArrowCol[0];
        }
        if (buildingHighlighted && highTime > highTimer)
        {
            highTimer += Time.deltaTime;
        }
        else
        {
            line.enabled = false;
            buildingHighlighted = false;
            Highlight.gameObject.SetActive(false);
            highTimer = 0;
            buildingHighlighted = false;
        }
        if (Select.gameObject.activeSelf)
        {

            if (Highlight.gameObject.activeSelf == true && buildingHighlighted)
            {
                line.SetPosition(0, Select.position);
                line.SetPosition(line.positionCount-1, Highlight.position);
                Vector3 linePos;
                for (float i=1;i<line.positionCount-1;i++)
                {
                    linePos = Vector3.Lerp(Select.position, (Highlight.position), i / line.positionCount);
                    linePos.z = Vector3.Slerp(Select.position, (Highlight.position), i / line.positionCount).z;
                    line.SetPosition((int)i, linePos);
                }
                line.enabled = true;
            }
            else
            {
                line.enabled = false;
            }
        }
        else
        {
            line.enabled = false;
            buildingHighlighted = false;
            Highlight.gameObject.SetActive(false);
        }
    }

}
