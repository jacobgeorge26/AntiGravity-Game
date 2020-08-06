using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Movement : MonoBehaviour
{
    public Camera cam; //camera
    public MouseLook mouseLook = new MouseLook(); //look with mouse move - Unity script - see bottom of class 
    public GameObject target; //cursor target
    [HideInInspector] public float currentTargetSpeed = 10f; //how fast player moves - recommend 5-20 
    //would be cool to have user move more slowly and 'bob' in zero gravity
    [HideInInspector] public bool hit = false; //has player been hit by laser - needed to avoid issues in Collision script
    public bool airControl; // can the user control the direction that is being moved in the air

    private Rigidbody body; //player
    private Vector3 movementVector; //vector for movement velocity


    //we like to move it move it
    //he like to move it move it
   //she like to move it move it
   //we like to 
   //move it

    //code based on RigidbodyFPSController script 


    void Start()
    {
        //get player object
        body = GetComponent<Rigidbody>();
        //initialise mouseLook script
        mouseLook.Init(transform, cam.transform, target.transform);
    }

    public void Update()
    {
        //rotate player with camera - so with mouse move
        RotateView();

        //if player is able to choose new motion and they've clicked a new path
        if (Input.GetMouseButtonDown(0) && airControl)
        {
            //move 'down the camera' as this is the centre point 
            Vector3 mousePos = cam.transform.forward;
            //set this as new movement vector
            movementVector = mousePos;
            //actually move the player now
            move();
        }
    }

    public void Reset(Transform transform)
    {
        //reset player at specified Transform (eg when player hit by laser)
        //resets everything - duplicate of where used in other areas as attempt to fix bug caused before bool hit was set up
        //the duplicate stuff in playerHit can be removed but seems like a chill failsafe
        //use this for future features
        var targetRenderer = target.GetComponent<Renderer>();
        body.position = transform.position;
        body.rotation = transform.rotation;
        body.velocity = Vector3.zero;
        body.useGravity = true;
        targetRenderer.material.SetColor("_Color", Color.red);
        airControl = true;
    }


    private void move()
    {
        //somewhere - i dont know where - currentTargetSpeed is being changed in some way so without this line the player shoots forward really really quickly
        currentTargetSpeed = 10f;
        //set movement vector to reflect the speed wanted
        Vector3 desiredMove = movementVector;
        desiredMove.x = desiredMove.x * currentTargetSpeed;
        desiredMove.z = desiredMove.z * currentTargetSpeed;
        desiredMove.y = desiredMove.y * currentTargetSpeed;
        //apply this to the player
        body.velocity = desiredMove;
    }

    private void RotateView()
    {
        //a lot of this is from the original script but seems useful for extensions so left in

        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        //apply rotation
        mouseLook.LookRotation(transform, cam.transform, target.transform);

        if (airControl)
        {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            body.velocity = velRotation * body.velocity;
        }
    }


    //unity class used to allow user to look around using mouse - rotates player with the mouse and centres camera
    //mostly untouched apart from camera fixes
    //centres target in screen (essentially piggybacking off the MouseLook script to make it seem like it's a cursor
    //it's not, it't just anchored to the player and stays in the centre of the screen at all times, and the camera moves with mouse movements ;)
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private Quaternion m_TargetTargetRot;
        private bool m_cursorIsLocked = true;
        private float distanceFromCamera = 0.5f;

        public void Init(Transform character, Transform camera, Transform target)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            m_TargetTargetRot = m_CameraTargetRot;
        }


        public void LookRotation(Transform character, Transform camera, Transform target)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
            m_TargetTargetRot = m_CameraTargetRot;

            if (clampVerticalRotation)
            {
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
                m_TargetTargetRot = m_CameraTargetRot;
            }

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
                target.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
                target.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
            //updates target position
            Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));
            target.transform.position = centerPos;
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if (!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
