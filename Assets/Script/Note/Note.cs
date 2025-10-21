using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] protected float noteSpeed = .1f;

	public virtual void Start()
	{
		noteSpeed = NoteSpawner.GetInstance().GetNoteSpeed();
	}

	public virtual void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * noteSpeed;
    }

    public void SetSpeed(float speed) { noteSpeed = speed; }
}
