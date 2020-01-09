using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    Rigidbody rb;
    public GameObject camera;
    public Vector3 velocity;
    public Vector3 localVel;
    public Vector3 accel;

    public float pushForce;
    public float pushRadius;

    public float speed;
    public float jumpForce;
    public float gravity;
    public float decelRatio;

    int xInput;
    int zInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate(){

        rb.rotation = ((Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0)));

        if(Input.GetKeyDown("space")){
            rb.AddForce(Vector3.up*jumpForce);
        }

        xInput = zInput = 0;

        if(Input.GetKey("w")){
            zInput++;
        }
        if(Input.GetKey("s")){
            zInput--;
        }
        if(Input.GetKey("a")){
            xInput--;
        }
        if(Input.GetKey("d")){
            xInput++;
        }

        Vector3 force = new Vector3(xInput, 0, zInput);
        rb.AddRelativeForce(force.normalized*speed*Time.deltaTime);

        rb.AddForce(Vector3.down*gravity*Time.deltaTime);

        if(Input.GetMouseButtonDown(0)){
            Collider[] hits = Physics.OverlapSphere(camera.transform.position + pushRadius*camera.transform.forward, pushRadius);

            foreach(Collider c in hits){
                Mosher m = c.GetComponent<Mosher>();

                if(m != null){
                    Vector3 direction = (m.transform.position - transform.position).normalized;
                    m.velocity += new Vector3(direction.x, 0.5f , direction.z) * pushForce;
                    m.gravity = Mathf.Max(Mathf.Min(m.gravity, -15), -30f);
                }
            }
        }
    }
}


 /*
        if(zInput != 0){
            localVel.z = speed*zInput;
        }else{
            if(localVel.z > 0.01f){
                localVel.z = localVel.z*decelRatio;
            }else{
                localVel.z = 0;
            }
        }

        if(xInput != 0){
            localVel.x = speed*xInput;
        }else{
            if(localVel.x > 0.01f){
                localVel.x = localVel.x*decelRatio;
            }else{
                localVel.x = 0;
            }
        }

        float rotation = camera.transform.rotation.eulerAngles.y*Mathf.Deg2Rad;
        velocity.x = Mathf.Sin(rotation)*localVel.z + Mathf.Cos(rotation)*localVel.x;
        velocity.z = -Mathf.Sin(rotation)*localVel.x + Mathf.Cos(rotation)*localVel.z;
        */