// original by Mr. Animator
// adapted to C# by @torahhorse
// http://wiki.unity3d.com/index.php/Headbobber

using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour
{	
	public float bobbingAmount = 0.05f;
	public float  midpoint = 0.6f; 
	
    private MixscapePlayer _player;

    protected void Start()
    {
        _player = GetComponentInParent<MixscapePlayer>();
    }

    void Update ()
	{ 
	    float waveslice = 0.0f; 
	    float horizontal = Input.GetAxis("Horizontal"); 
	    float vertical = Input.GetAxis("Vertical"); 
	    
	    waveslice = Mathf.Sin(_player.FootstepPercentage * Mathf.PI * 2.0f); 
	    if (waveslice != 0f)
	    { 
	       float translateChange = waveslice * bobbingAmount; 
	       float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical); 
	       totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f); 
	       translateChange = totalAxes * translateChange;
	       
	       Vector3 localPos = transform.localPosition;
	       localPos.y = midpoint + translateChange * Time.timeScale; 
	       transform.localPosition = localPos;
	    } 
	    else
	    { 
	    	Vector3 localPos = transform.localPosition;
	    	localPos.y = midpoint; 
	    	transform.localPosition = localPos;
	    } 
	}
}
