using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastToPlane : MonoBehaviour
{
    [SerializeField] private Transform _planeObject;
    [SerializeField] private Transform _hitObject;

    private Vector3 _originObject;
    private Vector3 _throwObject;
    private bool _isBorderRefresh;

    void Start()
    {
    }

	
    // Update is called once per frame
    void Update()
	{
	}

    // Update is called once per frame
    public (bool IsBorderOn, Vector3 Position) ThrowRay(Vector3 originObject, Vector3 throwObject)
    {
		_originObject = originObject;
		_throwObject = throwObject;
        _isBorderRefresh = false;
        
        var plane = new Plane(_planeObject.up, _planeObject.position);
        var ray = new Ray(_originObject, _throwObject);
        
        
        // レイと平面との当たり判定
        // ヒットした場合はenterに平面までの距離が格納される
        Debug.DrawRay (ray.origin, ray.direction * 30f, Color.red, 0.5f, false);
        var isHit = plane.Raycast(ray, out var enter);
        
        // ヒットした場合のみオブジェクトを表示
        _hitObject.gameObject.SetActive(isHit);
        if (isHit)
        {
            // ヒットした場合は平面の位置に点を移動
            _hitObject.position = ray.GetPoint(enter);
            TestRangeOfPlane();
        }

		return (_isBorderRefresh, _hitObject.transform.position);
    }

    private void TestRangeOfPlane()
    {
        Vector3 defaultPosition = _hitObject.transform.localPosition;
        float planeSize = 10f;

        if (defaultPosition.x < planeSize * -0.5f ||
            defaultPosition.x > planeSize * 0.5f)
        {
            _hitObject.transform.localPosition = new Vector3(Mathf.Clamp(defaultPosition.x,
                planeSize * -0.5f,
                planeSize * 0.5f) , 0, _hitObject.transform.localPosition.z);
            _isBorderRefresh = true;
        }

        if (defaultPosition.z < planeSize * -0.5f ||
            defaultPosition.z > planeSize * 0.5f)
        {
            _hitObject.transform.localPosition = new Vector3(_hitObject.transform.localPosition.x , 0, Mathf.Clamp(defaultPosition.z,
                planeSize * -0.5f,
                planeSize * 0.5f));
            _isBorderRefresh = true;
        }
            
    }
    
    private bool GetBorderRefreshFlag()
    {
        return _isBorderRefresh;
    }
}
