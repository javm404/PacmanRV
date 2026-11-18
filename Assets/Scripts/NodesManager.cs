using UnityEngine;
using UnityEditor;

public class NodesManager : MonoBehaviour
{
	Transform[] children;
	int i = 0;
	void Start()
	{
		children = GetComponentsInChildren<Transform>();
	}	
	void OnDrawGizmos()
	{
		if(children == null){
			children = GetComponentsInChildren<Transform>();
		}
		Gizmos.color = Color.orange;
		foreach(Transform t in children){
			if(i == 0){
				i+=1;
				continue;
			} else {
				Handles.Label(t.position,t.name);
				Gizmos.DrawWireSphere(t.position, 4f);	
			}
			i+=1;
		}
	}
}
