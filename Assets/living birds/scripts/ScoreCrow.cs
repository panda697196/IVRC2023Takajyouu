using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Eagle_Navigation;
using static UnityEngine.GraphicsBuffer;

public class ScoreCrow : MonoBehaviour
{
    private GameObject _randomScoreTarget;

    public GameObject _randomScoreTargetStorage;
    public Vector3 _spwanCenter;

    private List<GameObject> _backCrowList = new List<GameObject>(1);
    private List<GameObject> _randomScoreTargetList = new List<GameObject>(1);

    [SerializeField] private int _scaredCrow; //逃げたカラスの数
    public int ScaredCrow => _scaredCrow;

    [SerializeField] private float radius; //カラスが出現するエリアの半径
    [SerializeField] private int _comebackCrow; //帰って来るカラスの数
    
    [SerializeField] private GameObject lookObject; //カラスの向く向きを決めるオブジェクト
    [SerializeField] private GameObject _scoreBoard; //スコアボード
    [SerializeField] private GameObject _eagle; //鷹
    [SerializeField] private GameObject _toTakeScore; //スコアボードを取りに行く振りのためのTarget
    [SerializeField] private GameObject _showScore; //スコアボードを配置する場所
    [SerializeField] private GameObject _eagleIdle;
    private GameObject CrowManager;
    CrowGenerater Crowgene;



    void RandomCirclePos(int a)
    {
        for (int i = 0; i < a; i++)
        {
            
            // 指定された半径の円内のランダム位置
            var circlePos = radius * Random.insideUnitCircle;
            // XZ平面で指定された半径、中心点の円内のランダム位置を計算
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefabをインスタンス化する
            GameObject newTarget = Instantiate(_randomScoreTarget, spawnPos, Quaternion.LookRotation(relativePos));
            //見やすいように生成したTargetをRandomScoreTargetStorageに格納
            newTarget.transform.parent = _randomScoreTargetStorage.transform;
            lb_Crow lbCrow = _backCrowList[i].GetComponent<lb_Crow>();
            lbCrow.SetTarget(newTarget);
            lbCrow.SetCrowState(lb_Crow.birdBehaviors.flyToTarget);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //TargetのPregfabの読み取り
        _randomScoreTarget = (GameObject)Resources.Load("Sphere");
        //TragetのPregfabから生成したオブジェの格納
        _randomScoreTargetStorage = GameObject.Find("RandomScoreTargetStorage");
        CrowManager = GameObject.Find("CrowManager");
        lb_CrowTrigger lbTrigger = new lb_CrowTrigger();
        Crowgene = CrowManager.GetComponent<CrowGenerater>();
        _scoreBoard.SetActive(false);
        var BoardRigidbody = _scoreBoard.GetComponent<Rigidbody>();
        BoardRigidbody.useGravity = false;
        var BoardCollider = _scoreBoard.GetComponent<BoxCollider>();
        BoardCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //逃げたカラスを数え，残ったカラスを記録する
            ScaredCrowNumber();
            //鷹のコライダーをオフにすることで，カラスが払われるのを防ぐ
            var EagleCharacterController = _eagle.GetComponent<CharacterController>();
            EagleCharacterController.enabled = false;
            //残ったカラスをスコアボード近くに飛ばす
            ScoreCrowPos();
            //鷹がボードを持ってくる
            EagleAndBoard();
        }

        if (_scoreBoard.activeInHierarchy)
        {
            var EagleNavi = _eagle.GetComponent<Eagle_Navigation>();
            float dis = Vector3.SqrMagnitude(_showScore.transform.position - _eagle.transform.position);
            if (dis<5f)
            {
                DropScoreBoard();
                EagleNavi.SetTarget(_eagleIdle);
                var EagleEdit = _eagle.GetComponent<Eagle_Edit>();
                EagleNavi.SetFlyState(Eagle_Navigation.FlyState.targetAround);
                Invoke(nameof(WaitFly), 5f);
            }
        }
    }

    public void WaitFly()
    {
        var EagleNavi = _eagle.GetComponent<Eagle_Navigation>();
        EagleNavi.SetTarget(_eagleIdle);
        EagleNavi.SetFlyState(Eagle_Navigation.FlyState.laud);
    }

    //カラスリスト内にある，鷹によって飛んだカラスを数えるメソッド
    public void ScaredCrowNumber()
    {
        int i = 0;
        _scaredCrow = 0;
        List<GameObject> _crowList = Crowgene.CrowList;
        foreach (GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                _scaredCrow++; //鷹によって飛んだカラスを数える
            }
            else
            {
                _backCrowList.Add(_crowList[i]); //追い払えなかったカラスを格納
            }
            i++;
        }
        _comebackCrow = _backCrowList.Count;
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void ScoreCrowPos()
    {
        RandomCirclePos(_comebackCrow);
    }

    public void EagleAndBoard()
    {
        var EagleNavi = _eagle.GetComponent<Eagle_Navigation>();
        EagleNavi.SetTarget(_toTakeScore);
        var EagleEdit = _eagle.GetComponent<Eagle_Edit>();
        EagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
        //EagleEdit.SetEagleState(Eagle_Edit.EagleState.Fly);
        EagleNavi.SetFlyState(Eagle_Navigation.FlyState.target);
        Invoke(nameof(ShowBoard),10f);
    }

    public void ShowBoard()
    {
        _scoreBoard.SetActive(true);
        var EagleNavi = _eagle.GetComponent<Eagle_Navigation>();
        EagleNavi.SetTarget(_showScore);
        var EagleEdit = _eagle.GetComponent<Eagle_Edit>();
        EagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
        EagleNavi.SetFlyState(Eagle_Navigation.FlyState.target);
    }

    public void DropScoreBoard()
    {
        _scoreBoard.transform.SetParent(null);
        var BoardCollider = _scoreBoard.GetComponent<BoxCollider>();
        BoardCollider.enabled = true;
        var BoardRigidbody = _scoreBoard.GetComponent<Rigidbody>();
        BoardRigidbody.useGravity = true;
    }
}
