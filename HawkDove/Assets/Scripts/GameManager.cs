using System.Collections;
using System.Collections.Generic;
using System.Linq;  
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public Vector3 center;
    public GameObject hawkPrefab;
    public GameObject dovePrefab;
    public GameObject foodPrefab;

    public int min;
    public int max;

    public TMP_InputField hawkNum;
    public TMP_InputField doveNum;
    public TMP_InputField foodNum;
    public TMP_Dropdown population;

    public TMP_InputField foodValue;
    public TMP_InputField injury;
    public TMP_InputField bluffing;
    public TMP_InputField baseReq;

    public Button btn_Start;
    public Button btn_Next;
    public Button btn_Stop;

    public int foodValueInt;
    public int injuryInt;
    public int bluffingInt;
    public int baseReqInt;

    private DD_DataDiagram m_DataDiagram;
    List<GameObject> lineList = new List<GameObject>();
    private float h = 0;

    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject dd = GameObject.Find("DataDiagram");
        if (null == dd)
        {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        AddALine();
        AddALine();
        AddALine();

        
    }

    private bool updateSwitch = false;
    private int updateIndex = 0;
    // Update is called once per frame
    void Update()
    {
        if (updateSwitch)
        {
            if (updateIndex < 100)
            {
                Btn_Next_Click();
                updateIndex++;
            }
            if (updateIndex == 100)
                updateSwitch = false;
        }
    }

    public void Btn_Start_Click()
    {
        //        HawkManager hawk = gameObject.GetComponentInParent<HawkManager>();
        //       hawk.isDead = true;
        GameObject hawk = GameObject.Find("Hawk(Clone)");
        while(hawk != null)
        {
            GameObject.DestroyImmediate(hawk);
            hawk = GameObject.Find("Hawk(Clone)");
        }

        GameObject dove = GameObject.Find("Dove(Clone)");
        while (dove != null)
        {
            GameObject.DestroyImmediate(dove);
            dove = GameObject.Find("Dove(Clone)");
        }

        GameObject food = GameObject.Find("Food(Clone)");
        while (food != null)
        {
            GameObject.DestroyImmediate(food);
            food = GameObject.Find("Food(Clone)");
        }


        for (int i = 0; i < System.Convert.ToInt32(hawkNum.text); i++)
            spawnPrefab(hawkPrefab);
            
        for(int i = 0; i < System.Convert.ToInt32(doveNum.text); i++)
            spawnPrefab(dovePrefab);

        for (int i = 0; i < System.Convert.ToInt32(foodNum.text); i++)
            spawnPrefab(foodPrefab);

        hawkNum.enabled = false;
        doveNum.enabled = false;
        foodNum.enabled = false;
        foodValue.enabled = false;
        injury.enabled = false;
        bluffing.enabled = false;
        baseReq.enabled = false;

        foodValueInt = System.Convert.ToInt32(foodValue.text);
        injuryInt = System.Convert.ToInt32(injury.text);
        bluffingInt = System.Convert.ToInt32(bluffing.text);
        baseReqInt = System.Convert.ToInt32(baseReq.text);
    }

    public GameObject spawnPrefab(GameObject prefab)
    {
        Vector3 pos = center + new Vector3(Random.Range(min, max), Random.Range(min, max), 0);

        return Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    public void Btn_Next_Click()
    {
        BroadcastMessage("Expiring");
        for (int i = 0; i < System.Convert.ToInt32(foodNum.text); i++)
            spawnPrefab(foodPrefab);

        List<GameObject> foods = new List<GameObject>();
        List<GameObject> hawks = new List<GameObject>();
        List<GameObject> doves = new List<GameObject>();
        List<GameObject> agents = new List<GameObject>();
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "Food(Clone)")
            {
                foods.Add(go);
            }
            else if (go.name == "Hawk(Clone)")
            {
                hawks.Add(go);
                agents.Add(go);
            }
            else if (go.name == "Dove(Clone)")
            {
                doves.Add(go);
                agents.Add(go);
            }
        }
        while (agents.Count > 0)
        {
            if (foods.Count > 0)
            {
                if (agents.Count > 1)
                {
                    int random1 = Random.Range(0, agents.Count - 1);
                    GameObject agent1 = agents[random1];
                    agents.RemoveAt(random1);
                    int random2 = Random.Range(0, agents.Count - 1);
                    GameObject agent2 = agents[random2];
                    agents.RemoveAt(random2);
                    if (hawks.Contains(agent1) && hawks.Contains(agent2))
                    {
                        HawkManager hm = agent1.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        HawkManager hm2 = agent2.transform.GetComponent<HawkManager>();
                        hm2.energy -= (injuryInt + baseReqInt);
                    }
                    else if (doves.Contains(agent1) && doves.Contains(agent2))
                    {
                        DoveManager dm = agent1.transform.GetComponent<DoveManager>();
                        dm.energy += (foodValueInt - bluffingInt - baseReqInt);
                        DoveManager dm2 = agent2.transform.GetComponent<DoveManager>();
                        dm2.energy -= (bluffingInt + baseReqInt);
                    }
                    else if (hawks.Contains(agent1) && doves.Contains(agent2))
                    {
                        HawkManager hm = agent1.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        DoveManager dm = agent2.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                    else
                    {
                        HawkManager hm = agent2.transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                        DoveManager dm = agent1.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                }
                else
                {
                    if (hawks.Contains(agents[0]))
                    {
                        HawkManager hm = agents[0].transform.GetComponent<HawkManager>();
                        hm.energy += (foodValueInt - baseReqInt);
                    }
                    else
                    {
                        DoveManager dm = agents[0].transform.GetComponent<DoveManager>();
                        dm.energy += (foodValueInt - baseReqInt);
                    }
                    agents.RemoveAt(0);
                }
                int random3 = Random.Range(0, foods.Count - 1);
                GameObject.DestroyImmediate(foods[random3]);
                foods.RemoveAt(0);
            }
            else
            {
                foreach(GameObject go in agents)
                {
                    if (hawks.Contains(go))
                    {
                        HawkManager hm = go.transform.GetComponent<HawkManager>();
                        hm.energy -= baseReqInt;
                    }
                    else
                    {
                        DoveManager dm = go.transform.GetComponent<DoveManager>();
                        dm.energy -= baseReqInt;
                    }
                }
                break;
            }
                
        }

        int howkCount = 0;
        int doveCount = 0;

        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "Hawk(Clone)")
            {
                howkCount++;
            }
            else if (go.name == "Dove(Clone)")
            {
                doveCount++;
            }
        }

        Debug.Log(howkCount + "; " + doveCount);

        if (null == m_DataDiagram)
            return;

        foreach (GameObject l in lineList)
        {
            m_DataDiagram.InputPoint(l, new Vector2(index++, Random.value * 4f));
        }
    }

    public void Btn_Complete_Click()
    {
        Btn_Start_Click();
        updateSwitch = true;
    }


    public void Btn_Stop_Click()
    {
        hawkNum.enabled = true;
        doveNum.enabled = true;
        foodNum.enabled = true;
        foodValue.enabled = true;
        injury.enabled = true;
        bluffing.enabled = true;
        baseReq.enabled = true;
    }

    public void reproduction(string agent, int energy)
    {
        if (agent == "Hawk")
        {
            HawkManager hm = spawnPrefab(hawkPrefab).GetComponent<HawkManager>();
            hm.energy = energy / 2;
        }
        else
        {
            DoveManager dm = spawnPrefab(dovePrefab).GetComponent<DoveManager>();
            dm.energy = energy / 2;
        }

    }
    public void Expiring()
    {

    }

    void AddALine()
    {

        if (null == m_DataDiagram)
            return;

        Color color = Color.HSVToRGB((h += 0.1f) > 1 ? (h - 1) : h, 0.8f, 0.8f);
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }

    private void WriteToCSV(string fileName, string howkCount, string doveCount, string totalCount)
    {
        StringBuilder sb = new StringBuilder();
        string result = "";
        FileStream file = null;
        if (!File.Exists(fileName))
        {
            file = new FileStream(fileName, FileMode.CreateNew);
            sb.Append("Howk");
            sb.Append("Dove");
            sb.Append("Total");
            result = sb.ToString().Substring(0, sb.ToString().Length - 2);
        }
        else
        {
            file = new FileStream(fileName, FileMode.Append);
        }

        StreamWriter sw = new StreamWriter(file);
        if(result != "")
        {
            sw.WriteLine(result);
        }

        sb.Clear();

        sb.Append(howkCount);
        sb.Append(doveCount);
        sb.Append(totalCount);

        result = sb.ToString().Substring(0, sb.ToString().Length - 2);

        sw.WriteLine(result);
        sw.Flush();
        sw.Close();
    }

}
