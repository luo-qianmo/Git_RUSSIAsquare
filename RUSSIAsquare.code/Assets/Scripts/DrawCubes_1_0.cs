/*
 * @author luo_qianmo
 * 
 * 
 * 
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DrawCubes_1_0 : MonoBehaviour
{
    public int squre_x, squre_y;
    public int width = 10, heigh = 22;
    public float[] pings = { 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f,
        0.28f, 0.23f, 0.2f, 0.18f, 0.16f, 0.14f, 0.12f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    public int dif = 10;
    public GameObject wall, cube, particle, Pause,GG;
    public Text text;
    public int grade;
    public bool canInstantiate = true;
    public int nowstate = 0, nextstate = 0;
    public float nowPing;
    public float sumTime;
    public bool[,] map, square;
    public bool pause = false;
    // Use this for initialization
    private GameObject[] gms;
    private float aa, ss, dd;
    xypoint point;
    void Start()
    {
        nowPing = pings[0];
        point = GetComponent<xypoint>();
        map = new bool[100, 100];
        square = new bool[5, 5];
        for (int i = 0; i < width; i++) {
            map[i, 0] = true;
        }
        for (int i = 0; i < 100; i++) {
            map[0, i] = map[width, i] = true;
        }
        Draw();
        squre_x = width / 2 - 1;
        squre_y = heigh - 4;
        nextstate = Random.Range(0, 28);
        sumTime = nowPing;
    }

    // Update is called once per frame
    void Update()
    {
        nowPing =pings[grade/dif];
        if (Input.GetKeyDown(KeyCode.P)) {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            pause = !pause;
        }
        if (!pause) {
            Pause.SetActive(false);
            text.text = grade.ToString();
            sumTime += Time.deltaTime;
            aa += Time.deltaTime;
            ss += Time.deltaTime;
            dd += Time.deltaTime;
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && aa > 0.1f) {
                TurnLeft();
                aa = 0;
            }
            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && dd > 0.05f) {
                TuenRight();
                dd = 0;
            }
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && ss > 0.1f) {
                TurnDowm();
                ss = 0;
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                Rolate();
            }
            OverFlow();
            if (sumTime >= nowPing) {
                sumTime = 0;
                if (canInstantiate) {//需要生成的话就生成随机块
                    CreatNew();
                }
                else {
                    bool change = false;
                    for (int i = 0; i < 4; i++) {//遍历是否触地
                        for (int j = 0; j < 4; j++) {
                            if (square[i, j] && map[squre_x + i, squre_y + j - 1]) {
                                change = true;
                            }
                        }
                    }
                    if (change) {//触地就变色
                        OnFloor();
                    }
                    else {//否则下降一格
                        squre_y--;
                    }
                }
                Draw();
            }
        }
        else {
            Pause.SetActive(true);
        }
    }

    private void CreatNew()
    {
        aa = ss = dd = 0;
        print("nowstate: " + nowstate + " next: " + nextstate);
        nowstate = nextstate;
        nextstate = Random.Range(0, 28);
        DrawNextCube();
        square = new bool[5, 5];
        for (int i = 0; i < 4; i++) {
            square[point.x[nowstate * 4 + i], point.y[nowstate * 4 + i]] = true;
        }
        Draw();
        canInstantiate = false;
    }

    private void OnFloor()
    {
        for (int i = 0; i < 4; i++) {//变色
            for (int j = 0; j < 4; j++) {
                if (square[i, j]) {
                    map[squre_x + i, squre_y + j] = true;
                    square[i, j] = false;
                }
            }
        }
        squre_x = width / 2 - 1;
        squre_y = heigh -2;
        UpdateWall();
        canInstantiate = true;//可生成
        CreatNew();
    }

    private void TurnDowm()
    {
        bool change = true;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (square[i, j] && map[squre_x + i, squre_y + j - 1]) {
                    change = false;
                }
            }
        }
        if (change) {
            squre_y--;
            Draw();
        }
        else {
            OnFloor();
        }
    }

    private void TuenRight()
    {
        bool change = true;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (square[i, j] && map[squre_x + i + 1, squre_y + j]) {
                    change = false;
                }
            }
        }
        if (change) {
            squre_x++;
            Draw();
        }
    }
    private void TurnLeft()
    {
        bool change = true;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (square[i, j] && map[squre_x + i - 1, squre_y + j]) {
                    change = false;
                }
            }
        }
        if (change) {
            squre_x--;
            Draw();
        }
    }
    void Draw()
    {
        DrawWall();
        DrawCube();
    }
    void DrawWall()
    {
        gms = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject x in gms)
            Destroy(x);
        for (int i = 0; i <= width; i++) {
            for (int j = 0; j < 100; j++) {
                if (map[i, j])
                    Instantiate(wall, new Vector3(i, j, 0), Quaternion.identity);
            }
        }

    }
    void DrawCube()
    {
        gms = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject x in gms)
            Destroy(x);
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (square[i, j])
                    Instantiate(cube, new Vector3(squre_x + i, squre_y + j, 0), Quaternion.identity);
            }
        }
    }
    void UpdateWall()
    {
        CheckGrade();
        KillSpace();
    }
    private void CheckGrade()
    {
        int num;
        int time = 0;
        for (int j = 1; j < heigh; j++) {
            num = 0;
            for (int i = 1; i < width; i++) {
                if (map[i, j])
                    num++;
            }
            if (num == width - 1) {
                //积分
                time++;
                grade += time;
                //特效
                Instantiate(particle, new Vector3(5, j, -2), Quaternion.identity);
                for (int i = 1; i < width; i++) {
                    map[i, j] = false;
                }
            }
        }
    }
    private void KillSpace()
    {
        int num, times = 0;
        for (int j = 1; j < heigh; j++) {
            num = 0;
            for (int i = 1; i < width; i++) {
                if (map[i, j])
                    num++;
            }
            if (num == 0) {
                for (int k = j; k < heigh; k++)
                    for (int i = 1; i < width; i++) {
                        map[i, k] = map[i, k + 1];
                    }
                j--;
                times++;
                if (times == heigh) {
                    return;
                }
            }
        }
    }
    void Rolate()
    {
        bool change = true;
        int tmp = nowstate + 1;
        if (tmp % 4 == 0)
            tmp -= 4;
        for (int i = 0; i < 4; i++) {
            if (map[squre_x + point.x[tmp * 4 + i], squre_y + point.y[tmp * 4 + i]]) {
                change = false;
            }
        }
        if (change) {
            square = new bool[5, 5];
            for (int i = 0; i < 4; i++)
                square[point.x[tmp * 4 + i], point.y[tmp * 4 + i]] = true;
            nowstate = tmp;
            Draw();
        }
    }
    void DrawNextCube()
    {
        gms = GameObject.FindGameObjectsWithTag("Next");
        foreach (GameObject x in gms)
            Destroy(x);
        for (int i = 0; i < 4; i++) {
            GameObject x = Instantiate(cube,
                new Vector3(-5 + point.x[nextstate * 4 + i],
                15 + point.y[nextstate * 4 + i], -2),
                Quaternion.identity
                );
            x.tag = "Next";
        }
    }
    void OverFlow()
    {
        int num = 0;
        for (int i = 1; i < width; i++) {
            if (map[i, heigh-2])
                num++;
        }
        if (num >0)
            GG.SetActive(true);
    }
}