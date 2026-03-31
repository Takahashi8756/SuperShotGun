using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    move,
    fall,
    knockback,
    roadKill,
    Wait
}

public class EnemyMove : MonoBehaviour
{
    //実装確定している敵：ゆっくり近づいてくる敵と弾を撃ってくる敵
    //みんな移動速度は遅め、弾を撃ってくる敵は、プレイヤーと一定の距離感を維持しようとする。

    //変数はインスペクターから書き換えるためあえて子オブジェクトでは記述しない

    protected GameObject _playerObject = default; //プレイヤーのオブジェクト用の変数
    protected  readonly string PLAYERTAGNAME = "Player";//プレイヤーのタグの名前の変数
    //プレイヤー座標を入れるTransform
    protected Transform _target = default;
    protected NavMesh2DAgent _agent; //NavMeshAgent2Dを使用するための変数
    private PlayerStateManager _playerStateManager;
    [SerializeField, Header("盾持ち以外では変更しても意味ない")]
    protected float _moveSpeed = 5;//移動速度
    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }
    [SerializeField, Header("プレイヤーと取る距離、ロックオン以外が変えても意味ない")]
    protected float _keepDistance = 0;//プレイヤーと取る距離
    //移動不可能のフラグ
    protected bool _cantMove = false;
    //現在のステート
    protected EnemyState _enemyState = EnemyState.move;
    public EnemyState EnemyState
    {
        get { return _enemyState; }
        set { _enemyState = value; }//主にEnemyTakeDamageを継承したスクリプト達から書き換えてる
    }
    //向かう方向
    protected Vector2 _direction = default;
    public Vector2 Direction
    {
        get { return _direction; }
    }
    private bool _isFloating = false;
    public bool IsFloating
    {
        get { return _isFloating; }
    }

    private Rigidbody2D _rigidBody = default;

    private Vector2 _saveDirection = Vector2.zero;

    [SerializeField, Header("滑る力")]
    private float _inertiaStrangth = 4;

    public virtual void Start()
    {
        //プレイヤーのオブジェクトを探索して代入
        _playerObject = GameObject.FindWithTag(PLAYERTAGNAME);
        _playerStateManager = _playerObject.GetComponent<PlayerStateManager>();
        _target = _playerObject.transform;
        _agent = GetComponent<NavMesh2DAgent>(); //agentにNavMeshAgent2Dを取得
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void ChangeFloat(bool value)
    {
        _isFloating = value;
    }

    public void RoadKill()
    {
        _enemyState = EnemyState.roadKill;
    }
    private void FixedUpdate()
    {
        _cantMove = _enemyState == EnemyState.knockback || _enemyState == EnemyState.fall || _enemyState == EnemyState.Wait;
        if (_cantMove) //ノックバック中または落下中は移動しないようにする
        {
            return;
        }
        Moving();//移動のメソッド呼び出し
    }

    public virtual void Moving()
    {
        if(_target == null)
        {
            return;
        }
        if(_isFloating)
        {
            Debug.Log("あっ滑るッ！");
            float t = Time.fixedDeltaTime * _inertiaStrangth;
            _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity,_saveDirection, t);

        }
        else
        {
            _agent.destination = _target.position; //agentの目的地をtargetの座標にする
            _saveDirection = (_target.position - transform.position) * _moveSpeed;
        }

    }
}
