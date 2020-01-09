using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosher : MonoBehaviour
{
    public MoshBin bin;
    public float phase;
    public float floorHeight;
    public Vector2 anchorPoint;

    public Vector3 velocity;
    public float gravity;

    public int outputBin;

    //public float bouncePeriodError = 0;
    //private float lastBounceTime = -1;
    //private float[] bounceTimes = new float[64];
    //private int bounceCount = 0;

    public void Initialize(MoshBin b, float p, float g, float f){
    	bin = b;
        phase = p;
        gravity = g;
        floorHeight = f;
        anchorPoint = new Vector2(transform.position.x, transform.position.z);
    }

    public bool MosherUpdate(){
    	transform.position += velocity*Time.deltaTime;

    	if(transform.position.y > floorHeight){
    		//Apply gravity
    		velocity.y += gravity*Time.deltaTime;
    		return false;

    	}else{
            //Update output bin
            if(transform.position.x > 0){
                outputBin = (int)((bin.pit.binCenterOffset - Mathf.Atan((transform.position.z+bin.pit.binFocus) / transform.position.x)) / bin.pit.angleSpacing_rad);
            }else{

            }
            

            //Update personal gravity
            gravity = bin.gravity;

            //Jump
    		velocity.y = bin.jumpSpeed+(gravity*Time.deltaTime);

    		//Interpolate when the jump would have occurred between frames and adjust height accordingly.
    		float clipDist = floorHeight - transform.position.y;
    		transform.position = new Vector3(transform.position.x, floorHeight+clipDist, transform.position.z);

    		//Phase autocorrect
            //Phase is in units of time
    		float phaseError = (Time.time + phase) % bin.period;
    		if(phaseError > bin.period/2f){
    			phaseError = phaseError - bin.period;
    		}

    		velocity.y += (phaseError*bin.pit.phaseRestoreCoeff*gravity);

    		//Horizontal restoring motion
    		Vector2 horizRestore = new Vector2(anchorPoint.x-transform.position.x, anchorPoint.y-transform.position.z)*bin.pit.horizRestoreCoeff/bin.period;
			if(horizRestore.sqrMagnitude > bin.pit.horizRestoreMax){
				horizRestore = Vector2.ClampMagnitude(horizRestore, bin.pit.horizRestoreMax);
			}
    		velocity.x = horizRestore.x;
    		velocity.z = horizRestore.y;

            return true;
    	}
    }
}
