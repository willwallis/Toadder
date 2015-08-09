using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficSystemIntersection : MonoBehaviour 
{
	public enum IntersectionSystem
	{
		SINGLE = 0,                      // SINGLE = one light will turn green at any one time and cycle through each light in the m_lights container.
		DUAL   = 1                       // DUAL   = all the lights in m_lightsA will turn green and then all the lights in m_lightsB will turn green allow for multi lane movement within an intersection
	}

	public  bool                         m_automatic        = true;                                              // This allows users to toggle the traffic intersection (lights) manually if they choose too. Auto is default so they don't have to worry about it, just place in the editor and away they go.


	public  IntersectionSystem           m_intersectionSystem = IntersectionSystem.SINGLE;                       // SINGLE = one light will turn green at any one time and cycle through each light in the m_lights container.
	                                                                                                             // DUAL   = all the lights in m_lightsA will turn green and then all the lights in m_lightsB will turn green allow for multi lane movement within an intersection


	public  List<TrafficSystemTrafficLight>  m_lights       = new List<TrafficSystemTrafficLight>();             // these are found automatically on start. You can however put them in manually if you really want too but you should comment out Refresh() in the Awake() section if that is the case.
	public  List<TrafficSystemTrafficLight>  m_lightsA      = new List<TrafficSystemTrafficLight>();             // used only with DUAL Intersection System... see above comments on m_interactiveSystem
	public  List<TrafficSystemTrafficLight>  m_lightsB      = new List<TrafficSystemTrafficLight>();             // used only with DUAL Intersection System... see above comments on m_interactiveSystem
	private List<TrafficSystemTrafficLight>  m_priorityLightQueue      = new List<TrafficSystemTrafficLight>();
	private bool                             m_lightsAGreen     = true;                                          // used only with DUAL Intersection System... This is a toggle to know when to turn all the m_lightsA or m_lightsB green
	public  int                              m_lightIndex       = 0;
	public  bool                             m_activateOnStart  = true;
	private bool                             m_active           = false;
	public  float                            m_yellowDuration   = 2.0f;                                          // FYI, the green duration is on an individual light (TrafficSystemTrafficLight.cs)
	public  float                            m_redDuration      = 2.0f;
	public  bool                             m_ignoreCanFitAcrossIntersectionCheck = true;                       // If you want vehicles to stop and check that the intersection is clear, turn this on... will help prevent vehicles getting stuck in the middle of the intersection

	void Awake()
	{
		Refresh();
	}

	void Start () 
	{
		if(m_lightIndex >= m_lights.Count)
			m_lightIndex = 0;

		if(m_activateOnStart)
			ActivateLights();
	}

	public void AddToPriorityLightQueue( TrafficSystemTrafficLight a_light )
	{
		bool foundLight = false;
		for(int lIndex = 0; lIndex < m_priorityLightQueue.Count; lIndex++)
		{
			TrafficSystemTrafficLight light = m_priorityLightQueue[lIndex];

			if(light == a_light)
			{
				foundLight = true;
				break;
			}
		}

		if(!foundLight)
			m_priorityLightQueue.Add(a_light);
	}

	public void ActivateLights()
	{
		if(!m_active)
		{
			m_active = true;

			if(m_automatic)
				StartCoroutine( ActivateLightsAuto() ) ;
			else
				StartCoroutine( ActivateLightsManually() ) ;
		}
	}

	IEnumerator ActivateLightsAuto () 
	{
		while(m_active)
		{
			switch(m_intersectionSystem)
			{
			case IntersectionSystem.DUAL:
			{
				if(m_lightsAGreen)
				{
					for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
						m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );
					for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
						m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );

					if(m_lightsA.Count > 0)
						yield return new WaitForSeconds(m_lightsA[0].m_greenDuration);
				}
				else
				{
					for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
						m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
					for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
						m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );

					if(m_lightsB.Count > 0)
						yield return new WaitForSeconds(m_lightsB[0].m_greenDuration);
				}

				if(m_lightsAGreen)
				{
					for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
						m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
				}
				else
				{
					for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
						m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
				}
				yield return new WaitForSeconds(m_yellowDuration);
				
				if(m_lightsAGreen)
				{
					for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
						m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				}
				else
				{
					for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
						m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				}
				yield return new WaitForSeconds(m_redDuration);
				m_lightsAGreen = !m_lightsAGreen;

				if(m_priorityLightQueue.Count > 0)
				{
					m_intersectionSystem = IntersectionSystem.SINGLE;

					for(int lIndex = 0; lIndex < m_priorityLightQueue.Count; lIndex++)
						m_priorityLightQueue[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED, true );
					
					m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.GREEN, true );
					yield return new WaitForSeconds(m_priorityLightQueue[0].m_greenDuration);
					
					m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.YELLOW, true );
					yield return new WaitForSeconds(m_yellowDuration);
					
					m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.RED, true );
					yield return new WaitForSeconds(m_redDuration);

					m_priorityLightQueue.RemoveAt(0);

					yield return null;
				}

				m_intersectionSystem = IntersectionSystem.DUAL;
			}
				break;
			case IntersectionSystem.SINGLE:
			{
				if(m_lightIndex < m_lights.Count)
				{
					for(int lIndex = 0; lIndex < m_lights.Count; lIndex++)
						m_lights[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
					
					m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );
					yield return new WaitForSeconds(m_lights[m_lightIndex].m_greenDuration);
					
					m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
					yield return new WaitForSeconds(m_yellowDuration);
					
					m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
					yield return new WaitForSeconds(m_redDuration);
					
					m_lightIndex++;
					if(m_lightIndex >= m_lights.Count)
						m_lightIndex = 0;
				}
			}
				break;
			}

			yield return null;
		}

		yield return null;
	}

	IEnumerator ActivateLightsManually()
	{
		switch(m_intersectionSystem)
		{
		case IntersectionSystem.DUAL:
		{
			if(m_lightsAGreen)
			{
				for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
					m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );
				for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
					m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				
				if(m_lightsA.Count > 0)
					yield return new WaitForSeconds(m_lightsA[0].m_greenDuration);
			}
			else
			{
				for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
					m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
					m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );
				
				if(m_lightsB.Count > 0)
					yield return new WaitForSeconds(m_lightsB[0].m_greenDuration);
			}
			
			if(m_lightsAGreen)
			{
				for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
					m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
			}
			else
			{
				for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
					m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
			}
			yield return new WaitForSeconds(m_yellowDuration);
			
			if(m_lightsAGreen)
			{
				for(int lIndex = 0; lIndex < m_lightsA.Count; lIndex++)
					m_lightsA[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
			}
			else
			{
				for(int lIndex = 0; lIndex < m_lightsB.Count; lIndex++)
					m_lightsB[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
			}
			yield return new WaitForSeconds(m_redDuration);
			m_lightsAGreen = !m_lightsAGreen;

			// TODO: if you want to manually process the priority queue for vehicles turning you'll need to put a bool around the next if statement and toggle that on and off.
			//       Currently it will automatically let the cars turn left after the above code (both lanes turn red).

			if(m_priorityLightQueue.Count > 0)
			{
				m_intersectionSystem = IntersectionSystem.SINGLE;
				
				for(int lIndex = 0; lIndex < m_priorityLightQueue.Count; lIndex++)
					m_priorityLightQueue[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED, true );
				
				m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.GREEN, true );
				yield return new WaitForSeconds(m_priorityLightQueue[0].m_greenDuration);
				
				m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.YELLOW, true );
				yield return new WaitForSeconds(m_yellowDuration);
				
				m_priorityLightQueue[0].SetStatus( TrafficSystemTrafficLight.Status.RED, true );
				yield return new WaitForSeconds(m_redDuration);
				
				m_priorityLightQueue.RemoveAt(0);
				
				yield return null;
			}


			
			m_intersectionSystem = IntersectionSystem.DUAL;
		}
			break;
		case IntersectionSystem.SINGLE:
		{
			if(m_lightIndex < m_lights.Count)
			{
				for(int lIndex = 0; lIndex < m_lights.Count; lIndex++)
					m_lights[lIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				
				m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.GREEN );
				yield return new WaitForSeconds(m_lights[m_lightIndex].m_greenDuration);
				
				m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.YELLOW );
				yield return new WaitForSeconds(m_yellowDuration);
				
				m_lights[m_lightIndex].SetStatus( TrafficSystemTrafficLight.Status.RED );
				yield return new WaitForSeconds(m_redDuration);
				
				m_lightIndex++;
				if(m_lightIndex >= m_lights.Count)
					m_lightIndex = 0;
			}
		}
			break;
		}

		yield return null;
		m_active = false;
	}

	public void Refresh()
	{
		FindAllLights();
	}
	
	void FindAllLights()
	{
		m_lights.Clear();

		FindAllLightsRecursive(this, gameObject);
	}
	
	void FindAllLightsRecursive(TrafficSystemIntersection a_trafficSystemIntersection, GameObject a_obj)
	{
		if(a_obj)
		{
			if(a_obj.GetComponent<TrafficSystemTrafficLight>())
			{
				TrafficSystemTrafficLight light = a_obj.GetComponent<TrafficSystemTrafficLight>();
				a_trafficSystemIntersection.m_lights.Add(light);
			}
			
			for(int cIndex = 0; cIndex < a_obj.transform.childCount; cIndex++)
			{
				Transform child = a_obj.transform.GetChild(cIndex);
				if(child.gameObject)
					FindAllLightsRecursive(a_trafficSystemIntersection, child.gameObject);
			}
		}
	}

}
