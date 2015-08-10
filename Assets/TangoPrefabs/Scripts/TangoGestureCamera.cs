﻿//-----------------------------------------------------------------------
// <copyright file="TangoGestureCamera.cs" company="Google">
//   
// Copyright 2015 Google Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Tango;

/// <summary>
/// Pointcloud orbit camera.
/// </summary>
public class TangoGestureCamera : MonoBehaviour
{
    /// <summary>
    /// Camera type enum.
    /// </summary>
    public enum CameraType
    {
        FIRST_PERSON = 0x1,
        THIRD_PERSON = 0x2,
        TOP_DOWN = 0x4
    }
    
    public GameObject m_targetFollowingObject;

    // Set this to enable the First / Third / Top UI buttons.
    public bool m_enableCameraModeUI = false;

    // The default camera mode.
    public CameraType m_defaultCameraMode = CameraType.FIRST_PERSON;
    
    private Vector3 m_curOffset;
    
    private Vector3 m_firstPersonCamOffset = Vector3.zero;
    private Vector3 m_thirdPersonCamOffset = new Vector3(0.0f, 3.0f, -3.0f);
    private Vector3 m_topDownCamOffset = new Vector3(0.0f, 7.0f, 0.0f);
    
    private CameraType m_currentCamera;
    
    private float curThirdPersonRotationX = 180.0f;
    private float curThirdPersonRotationY = 0.0f;

    private float startThirdPersonRotationX = 45.0f;
    private float startThirdPersonRotationY = -45.0f;

    private float startThirdPersonCameraCircleR = 0.0f;
    private float curThirdPersonCameraCircleR = 7.0f;

    private Vector2 touchStartPoint = Vector2.zero;
    private float topDownStartY = 0.0f;
    
    private float touchStartDist = 0.0f;
    
    private Vector2 topDownStartPos = Vector2.zero;
    private Vector3 thirdPersonCamStartOffset;

	// Score, Lives, and Time.
	public int lives = 0;
	public Text liveText;
	public Text winMessage;

	// Scale Movement
	public float m_movementScale = 1.0f;
	
	// Determine if Toad is hit by a vehicle
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Vehicle")) {
			lives = lives - 1;
			setLives ();
			winMessage.text = "SMASH";
		}
	}

	void OnTriggerExit (Collider other) {
		winMessage.text = "";
	}


    /// <summary>
    /// Enabled based on camera type.
    /// </summary>
    /// <param name="cameraType">Enable which camera.</param>
    public void EnableCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.FIRST_PERSON:
            {
     			transform.localPosition = m_targetFollowingObject.transform.position;
                transform.localRotation = m_targetFollowingObject.transform.rotation;
                break;
            }
            case CameraType.THIRD_PERSON:
            {
                startThirdPersonRotationX = 45.0f;
                startThirdPersonRotationY = -45.0f;
                startThirdPersonCameraCircleR = 0.0f;
                curThirdPersonCameraCircleR = 7.0f;
                curThirdPersonRotationX = startThirdPersonRotationX + (Mathf.PI * (0.0f / Screen.width));
                curThirdPersonRotationY = startThirdPersonRotationY + (Mathf.PI * (0.0f / Screen.height));
                
                Vector3 newPos = new Vector3(curThirdPersonCameraCircleR * Mathf.Sin(curThirdPersonRotationX),
                                             curThirdPersonCameraCircleR * Mathf.Cos(curThirdPersonRotationY),
                                             curThirdPersonCameraCircleR * Mathf.Cos(curThirdPersonRotationX));
                m_curOffset = m_thirdPersonCamOffset = newPos;
                
                transform.position = m_targetFollowingObject.transform.position + m_curOffset;
                transform.LookAt(m_targetFollowingObject.transform.position);
                break;
            }
            case CameraType.TOP_DOWN:
            {
                m_topDownCamOffset = new Vector3(0.0f, 7.0f, 0.0f);
                break;
            }
        }
        m_currentCamera = cameraType;
    }
    
    /// <summary>
    /// Set up cameras.
    /// </summary>
    private void Start() 
    {
        Application.targetFrameRate = 60;
		setLives ();
		EnableCamera(m_defaultCameraMode);
    }

	private void setLives() {
		liveText.text = "Lives: " + lives.ToString("D4");
	}
		
		/// <summary>
    /// Updates, take touching event.
    /// </summary>
    private void LateUpdate()
    {
        if (m_currentCamera == CameraType.FIRST_PERSON)
        {
			// Get the pose position (x, y, and z)
			float poseX = m_targetFollowingObject.transform.position.x;
			float poseY = m_targetFollowingObject.transform.position.y;
			float poseZ = m_targetFollowingObject.transform.position.z;
			
			// Create a vector based on pose X and Z (we don't want player going up and down) 
			// Multiply by multiplier, this is because in small room not enough space to move.
			Vector3 posePosition = new Vector3 (poseX * m_movementScale, poseY * m_movementScale, poseZ * m_movementScale);

//			transform.localPosition = m_targetFollowingObject.transform.position; 
			transform.localPosition = posePosition;
			transform.localRotation = m_targetFollowingObject.transform.rotation; 
        }

        if (m_currentCamera == CameraType.THIRD_PERSON)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPoint = Input.GetTouch(0).position;
                startThirdPersonRotationX = curThirdPersonRotationX;
                startThirdPersonRotationY = curThirdPersonRotationY;
            }
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 offset = Input.touches[0].deltaPosition;
                curThirdPersonRotationX += -offset.y;
                curThirdPersonRotationY += offset.x;
                curThirdPersonRotationX = Mathf.Clamp(curThirdPersonRotationX, -89, 89);
            }
            
            if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began)
            {
                startThirdPersonCameraCircleR = curThirdPersonCameraCircleR;
                touchStartDist = Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x) +
                    Mathf.Abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y);
            }
            if (Input.touchCount == 2 && 
                (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
            {
                float curTouchDist = Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x) +
                    Mathf.Abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y);
                
                float tmp = 10 * (touchStartDist - curTouchDist) / (Screen.width + Screen.height);
                curThirdPersonCameraCircleR = startThirdPersonCameraCircleR + tmp;
                curThirdPersonCameraCircleR = Mathf.Clamp(curThirdPersonCameraCircleR, 0.5f, 20.0f);
            }
            
            m_thirdPersonCamOffset = Quaternion.Euler(curThirdPersonRotationX, curThirdPersonRotationY, 0.0f) * new Vector3(0.0f, 0.0f, -curThirdPersonCameraCircleR);
            m_curOffset = m_thirdPersonCamOffset;
            transform.position = m_targetFollowingObject.transform.position + m_curOffset;
            transform.LookAt(m_targetFollowingObject.transform.position);
        }

        if (m_currentCamera == CameraType.TOP_DOWN)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPoint = Input.GetTouch(0).position;
                topDownStartPos = new Vector2(m_topDownCamOffset.x, 
                                              m_topDownCamOffset.z);
            }
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 offset = Input.GetTouch(0).position - touchStartPoint;
                Vector2 curPos = topDownStartPos - (offset / 300.0f);
                
                Vector3 newPos = new Vector3(curPos.x, m_topDownCamOffset.y, curPos.y);
                m_topDownCamOffset = newPos;
            }

            if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began)
            {
                touchStartDist = Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x) +
                    Mathf.Abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y);
                topDownStartY = m_topDownCamOffset.y;
            }
            if (Input.touchCount == 2 && 
                (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
            {
                float curTouchDist = Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x) +
                    Mathf.Abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y);
                float offset = 10.0f * (touchStartDist - curTouchDist) / (Screen.width + Screen.height);
                Vector3 newPos = new Vector3(m_topDownCamOffset.x, 
                                             Mathf.Clamp(topDownStartY + offset, 1.5f, 100.0f), 
                                             m_topDownCamOffset.z);
                
                m_topDownCamOffset = newPos;
            }
            if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended))
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    touchStartPoint = Input.GetTouch(1).position;
                    topDownStartPos = new Vector2(m_topDownCamOffset.x,
                                                  m_topDownCamOffset.z);
                }
                if (Input.GetTouch(1).phase == TouchPhase.Ended)
                {
                    touchStartPoint = Input.GetTouch(0).position;
                    topDownStartPos = new Vector2(m_topDownCamOffset.x,
                                                  m_topDownCamOffset.z);
                }
            }
            transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
            transform.position = m_targetFollowingObject.transform.position + m_topDownCamOffset;
        }
    }
    
    /// <summary>
    /// Draw buttons to swap current behavior. 
    /// </summary>
    private void OnGUI()
    {
        if (!m_enableCameraModeUI)
        {
            return;
        }

        if (GUI.Button(new Rect(Screen.width - Common.UI_BUTTON_SIZE_X - Common.UI_BUTTON_GAP_X, 
                                Screen.height - ((Common.UI_BUTTON_SIZE_Y + Common.UI_LABEL_GAP_Y) * 3),
                                Common.UI_BUTTON_SIZE_X, 
                                Common.UI_BUTTON_SIZE_Y), "<size=20>First</size>"))
        {
            EnableCamera(CameraType.FIRST_PERSON);
        }
        if (GUI.Button(new Rect(Screen.width - Common.UI_BUTTON_SIZE_X - Common.UI_BUTTON_GAP_X, 
                                Screen.height - ((Common.UI_BUTTON_SIZE_Y + Common.UI_LABEL_GAP_Y) * 2),
                                Common.UI_BUTTON_SIZE_X, 
                                Common.UI_BUTTON_SIZE_Y), "<size=20>Third</size>"))
        {
            EnableCamera(CameraType.THIRD_PERSON);
        }
        if (GUI.Button(new Rect(Screen.width - Common.UI_BUTTON_SIZE_X - Common.UI_BUTTON_GAP_X, 
                                Screen.height - (Common.UI_BUTTON_SIZE_Y + Common.UI_LABEL_GAP_Y),
                                Common.UI_BUTTON_SIZE_X, 
                                Common.UI_BUTTON_SIZE_Y), "<size=20>Top</size>"))
        {
            EnableCamera(CameraType.TOP_DOWN);
        }
    }
}
