using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore; 	
#if UNITY_EDITOR
using input=GoogleARCore.InstantPreviewInput;
#endif
    public class ARController:MonoBehaviour{
	private List<DetectedPlane> planeDetected=new List<DetectedPlane>();
	public GameObject GridPrefab;
	public GameObject PortalDoor;
	public GameObject ARCamera;
	void Start(){
		}
	void Update()
    {
        //check AR Core tracking status
		if(Session.Status!=SessionStatus.Tracking)
        {
			return;
		}

        Session.GetTrackables<DetectedPlane>(planeDetected,TrackableQueryFilter.New); //fill List planeDetected with current planes

        //instatiate new grid for each detected plane
        for (int i=0;i<planeDetected.Count;++i)
			{
			GameObject grid= Instantiate(GridPrefab, Vector3.zero, Quaternion.identity, transform); 
			grid.GetComponent<GridVisualiser>().Initialize(planeDetected[i]);
			}

        //check if user has touched the screen
        Touch t;

        if (Input.touchCount<1||(t=Input.GetTouch(0)).phase!=TouchPhase.Began)
        {
			return;                         //return if not touched
        }
        //check if detected plane is touched or not
        TrackableHit hit;

        if (Frame.Raycast(t.position.x,t.position.y,TrackableHitFlags.PlaneWithinPolygon,out hit))
            {
	        PortalDoor.SetActive(true);                             //set portal door active
	        Anchor a=hit.Trackable.CreateAnchor(hit.Pose);          //create new anchor
	        PortalDoor.transform.position=hit.Pose.position;        //set position of portal
	        PortalDoor.transform.rotation=hit.Pose.rotation;        //set rotation of portal
	        Vector3 cameraPosition=ARCamera.transform.position;     //portal to face camera
	        cameraPosition.y=hit.Pose.position.y;                   //lock rotation of portal to Y axis
	        PortalDoor.transform.LookAt(cameraPosition,PortalDoor.transform.up);    //rotate portal to face camera  
	        PortalDoor.transform.parent=a.transform;                //attach portal to anchor
	        }
    }
}	
			
