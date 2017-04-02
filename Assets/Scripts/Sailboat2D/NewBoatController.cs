using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

BOAT CONTROLLER

Attach this to boat object. 
Accelerate and decelerate with Input.Vertical axis (default W/S or Up Arrow/Down Arrow).
Turn with Input.Horizontal axis (default A/D or Left Arrow/Right Arrow).

Wind has an angle from -pi to pi and a force. Set force to 0 for no wind. Wind will continually push the boat in some direction.
If the boat is stationary, then wind will not effect it. Wind only effects boat when boat is moving. If the boat moves with the
wind, it will speed up, or if the boat moves against the wind, it will slow down.

If you check 'Visualize Forces' under Wind category, then you'll notice a red line and a black line. The black line is the
wind force and the red line is the velocity of the ship. When going against the wind, the red line is shorter than when going with
the wind (the magnitude of this phenomenon is relative to the wind force).

*/

public class NewBoatController : MonoBehaviour {

    [System.Serializable]
    public class Sail {
        private float angle = 0.0f; //between 0 and 360
        [Range(0, 100)] public float strength = 0.0f;

        public float GetAngle() {
            return angle;
        }

        public void RotateSail(float degrees) {
            angle += degrees;
            if (angle < 0) {
                angle += 360;
            }else if (angle > 360) {
                angle -= 360;
            }
        }
    }

    private float QUICKRAD = 0.017453f;

    public BoatController.Movement movement;
    public Sail sail;
    public BoatController.Wind wind;

    private float speed;
    private float sailInput;
    private float turnInput;

    private bool docked = false;
    private Dock dock;

    Rigidbody2D rigidbody;
    Vector2 input;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }


    void Update() {
        print(sail.GetAngle());
        Vector3 sailLine = new Vector3(Mathf.Cos(this.sail.GetAngle() * QUICKRAD), Mathf.Sin(this.sail.GetAngle() * QUICKRAD), 0);
        Debug.DrawLine(-sailLine + transform.position, sailLine + transform.position, Color.magenta);
        if (docked) {
            if (Input.GetKeyDown("e")){
                docked = false;
            }
            LerpToDock();
        }
        else {
            if (Input.GetKeyDown("e")) {
                if (dock != null) {
                    docked = true;
                }
            }
            GetInput();
            CalcSail();
            TurnPhysics();
            MovementPhysics();
            CalcVelocity();
        }
       
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Dock")) {
            dock = other.gameObject.GetComponent<Dock>();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Dock")) {
            dock = null;
        }
    }

    private float AngleDifference(float two, float one) {
        float result = two - one;
        if (result > 180) {
            result -= 360;
        }
        else if(result < -180) {
            result += 360;
        }
        return result;
    }

    private void LerpToDock() {
        if (dock != null) {
            rigidbody.velocity = Vector2.zero;
            speed = 0;
            if ((transform.position - dock.boatPosition.position).sqrMagnitude > .01) {
                transform.position = Vector3.Lerp(transform.position, dock.boatPosition.position, 1.0f * Time.deltaTime);
            }
            float angleDifference = AngleDifference(transform.eulerAngles.z, dock.boatPosition.eulerAngles.z);
            if (angleDifference > .5f || angleDifference < -.5f) {
                float z = Mathf.Lerp(dock.boatPosition.eulerAngles.z, angleDifference, 1.0f * Time.deltaTime);
                transform.Rotate(0,0,-z);
            }
            float sailAngleDifference = AngleDifference(sail.GetAngle(), dock.boatPosition.eulerAngles.z);
            if (sailAngleDifference > .5f || sailAngleDifference < -.5f){
                float rotation = Mathf.Lerp(dock.boatPosition.eulerAngles.z, sailAngleDifference, 1.0f * Time.deltaTime);
                sail.RotateSail(-rotation);
            }
            turnInput = Mathf.LerpAngle(turnInput, sail.GetAngle(), 1.0f * Time.deltaTime);

        }

    }

    private void CalcSail() {
        float angleDifference = AngleDifference(transform.rotation.eulerAngles.z, sail.GetAngle());
        if (angleDifference > 45 && input.x > 0) {
            input.x = 0;
        }
        else if(angleDifference < -45 && input.x < 0) {
            input.x = 0;
        }

        //Slow it down faster than we speed it up
        if (Math.Abs(input.x) < .05f) {
            sailInput = Mathf.Lerp(sailInput, input.x, 2f * Time.deltaTime);
        }
        else {
            sailInput = Mathf.Lerp(sailInput, input.x, 1.5f * Time.deltaTime);
        }
        //Calculate an appropriate amount to turn
        this.sail.RotateSail(-sailInput * movement.sailSpeed * (1.0f + Time.deltaTime));
    }

    void TurnPhysics(){
        if (Math.Abs(rigidbody.velocity.sqrMagnitude) < 0.2f){
            return;
        }
        float previousTurnInput = turnInput;
        turnInput = Mathf.LerpAngle(turnInput, sail.GetAngle(), 1.0f * Time.deltaTime);
        float turnAmount = (turnInput - previousTurnInput) * movement.turnSpeed * (1.0f + Time.deltaTime);
        transform.Rotate(new Vector3(0, 0, turnAmount));
    }

    void MovementPhysics() {
        //Add acceleration to speed if we are moving forward.
        speed += input.y * movement.acceleration;
        //Subtract water friction from speed.
        speed -= movement.waterFriction;
        //Don't let speed go negative.
        speed = Mathf.Clamp(speed, 0.0f, movement.maxSpeed);
    }

    void CalcVelocity() {
        Vector3 velocity = transform.up * speed * (1.0f + Time.deltaTime);
        Vector3 windForce = new Vector3(
            Mathf.Sin(wind.angle) * wind.strength * Mathf.Clamp01(rigidbody.velocity.sqrMagnitude), 0.0f,
            Mathf.Cos(wind.angle) * wind.strength * Mathf.Clamp01(rigidbody.velocity.sqrMagnitude)
        );

        velocity += windForce;

        if (wind.visualizeForces) {
            Debug.DrawLine(transform.position, transform.position + windForce, Color.blue);
            Debug.DrawLine(transform.position, transform.position + velocity, Color.red);
        }

        rigidbody.velocity = velocity;
    }

    void GetInput() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }
}