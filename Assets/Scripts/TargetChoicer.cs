using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChoicer : MonoBehaviour
{
    [SerializeField] private RaycastToPlane _raycastTo1stPlane;
    [SerializeField] private GameObject _objectOf1stThrow;
    
    [SerializeField] private RaycastToPlane _raycastTo2ndPlane;
    [SerializeField] private GameObject _objectOf2ndThrow;

	[SerializeField] private Transform _hmdPosition;
	[SerializeField] private Transform _lookPosition;
	[SerializeField] private Transform _handPosition;
	[SerializeField] private Transform _throwPosition;

	[SerializeField] private int _backFrameToLook = 3;

	[SerializeField] private GameObject _debugPositionOfLook;
	[SerializeField] private GameObject _debugPositionOfVector;
	[SerializeField] private GameObject _debugPositionOfThrow;

	private float _span = 0.1f;
	private float _time = 0;

	
	private Vector3 _fixedHmdPosition;
	private Vector3 _fixedLookPosition;
	private Vector3 _fixedHandPosition;

	private Vector3 _fixedAfterHandPosition;
	private Vector3 _fixedThrowPosition;


	private Vector3[] _positionOfLookBack;
	private Vector3[] _positionOfHandBack;
    private Vector3 _target;
    
    // Start is called before the first frame update
    void Start()
    {
		_time = 0;
        _objectOf1stThrow.SetActive(false);
        _objectOf2ndThrow.SetActive(false);
		_positionOfLookBack = new Vector3[_backFrameToLook];
		_positionOfHandBack = new Vector3[_backFrameToLook];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OffTargetChoicePlane()
    {
        _objectOf1stThrow.SetActive(false);
        _objectOf2ndThrow.SetActive(false);
    }

    public void On1stTarget()
    {
        _objectOf1stThrow.SetActive(true);
    }
    public void On2ndTarget()
    {
        _objectOf2ndThrow.SetActive(true);
    }

	public void TraceTarget()
	{
		_time += Time.deltaTime;
		//視線によるターゲットと投げる直前の手の位置 _back時間まで記録する
		if(_time > _span)
		{
			_time = 0;
			for (int i = _backFrameToLook - 1; i > 0; i--)
			{
				_positionOfLookBack[i] = _positionOfLookBack[i - 1];
				_positionOfHandBack[i] = _positionOfHandBack[i - 1];
			}

			_positionOfLookBack[0] = _lookPosition.position;
			_positionOfHandBack[0] = _handPosition.position;
		}
	}


    public Vector3 Set1stTarget()
    {
		//候補から選ぶ
		//plane上に乗っているかのチェック
		var targetOfLook       = _raycastTo1stPlane.ThrowRay(_fixedHmdPosition, _fixedLookPosition); //返り値は(bool, position)
		var targetOfHandVector = _raycastTo1stPlane.ThrowRay(_fixedHandPosition, _fixedAfterHandPosition); //返り値は(bool, position)
		var targetOfThrow      = _raycastTo1stPlane.ThrowRay(_fixedHandPosition, _fixedThrowPosition); //返り値は(bool, position)

		_debugPositionOfLook.transform.position = targetOfLook.Position;
		_debugPositionOfVector.transform.position = targetOfHandVector.Position;
		_debugPositionOfThrow.transform.position = targetOfThrow.Position;
		
		//一つだけ枠内なら，それを採用
		if (targetOfLook.IsBorderOn && !targetOfHandVector.IsBorderOn && !targetOfThrow.IsBorderOn)
			return targetOfLook.Position;
		else if(!targetOfLook.IsBorderOn && targetOfHandVector.IsBorderOn && !targetOfThrow.IsBorderOn)
			return targetOfHandVector.Position;
		else if(!targetOfLook.IsBorderOn && !targetOfHandVector.IsBorderOn && targetOfThrow.IsBorderOn)
			return targetOfThrow.Position;

		//２つ枠内なら，その平均値を使用
		if (targetOfLook.IsBorderOn && targetOfHandVector.IsBorderOn && !targetOfThrow.IsBorderOn)
			return Get2Average(targetOfLook.Position, targetOfHandVector.Position);
		else if (!targetOfLook.IsBorderOn && targetOfHandVector.IsBorderOn && targetOfThrow.IsBorderOn)
			return Get2Average(targetOfThrow.Position, targetOfHandVector.Position);
		else if (!targetOfLook.IsBorderOn && targetOfHandVector.IsBorderOn && targetOfThrow.IsBorderOn)
			return Get2Average(targetOfThrow.Position, targetOfHandVector.Position);
		else if (targetOfLook.IsBorderOn && !targetOfHandVector.IsBorderOn && targetOfThrow.IsBorderOn)
			return Get2Average(targetOfThrow.Position, targetOfLook.Position);

		//3つ枠内，もしくは全て枠外なら，全ての平均値を使用
		return Get3Average(targetOfLook.Position, targetOfHandVector.Position, targetOfThrow.Position);
        //return _target;
    }


    public Vector3 Set2ndTarget()
    {
		//TODO:2回目を記述
		return _target;
	}

	public Vector3 Get2Average(Vector3 vector1, Vector3 vector2)
	{
		return (vector1 + vector2) / 2f;
	}

	public Vector3 Get3Average(Vector3 vector1, Vector3 vector2, Vector3 vector3)
	{
		return (vector1 + vector2 + vector3) / 3f;
	}

    public void DecideTarget()
    {
		//飛ばした瞬間のターゲット確定
		_fixedHmdPosition = _hmdPosition.position;
		_fixedLookPosition = _positionOfLookBack[_backFrameToLook];
		_fixedHandPosition = _positionOfHandBack[_backFrameToLook];
    }

	public void AfterDecideTarget()
	{
		//一定時間経過後に生まれる候補ターゲットの確定
 		_fixedAfterHandPosition = _handPosition.position;
		_fixedThrowPosition = _throwPosition.position;
	}

	public Vector3 SetTempTarget()
	{
		var targetOfLook       = _raycastTo1stPlane.ThrowRay(_fixedHmdPosition, _fixedLookPosition); //返り値は(bool, position)
		//仮のターゲットを視線の位置とする
		return targetOfLook.Position;
	}
}
