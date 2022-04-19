using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement : MonoBehaviour 
	{
		/// the possible movement types
		public enum PossibleMovementType
		{
			ConstantSpeed,
			EaseOut
		}

		/// the possible movement directions
		public enum MovementDirection
		{
			Ascending,
			Descending
		}

		[Header("Path")]
		/// if true, the path will loop and the object will keep on going in the same direction unless told otherwise
		public bool LoopPath=true;
		/// the points that make up the path the object will follow
		public List<Vector3> Path;
		/// the initial movement direction : ascending > will go from the points 0 to 1, 2, etc ; descending > will go from the last point to last-1, last-2, etc
		public MovementDirection InitialMovementDirection = MovementDirection.Ascending;

		[Header("Movement")]
		/// the movement speed
		public float Speed = 1;
		/// returns the current speed at which the object is traveling
		public Vector3 CurrentSpeed { get; protected set; }
		/// the movement type of the object
		public PossibleMovementType MovementType = PossibleMovementType.ConstantSpeed;

		[Header("Delay")]
		/// the list of delays (in seconds) to apply when the object meets each item of the path. Obviously you'll want to have the same number of items in the Delays list as in the Path list.
		public List<float> Delays;

		[Header("Settings")]
		/// the minimum distance to a point at which we'll arbitrarily decide the point's been reached
		public float MinDistanceToGoal = .1f;
		/// the original position of the transform, hidden and shouldn't be accessed
		public Vector3 _OriginalTransformPosition;
		/// internal flag, hidden and shouldn't be accessed
		public bool _OriginalTransformPositionSet=false;

		protected bool _active=false;
	    protected IEnumerator<Vector3> _currentPoint;
		protected int _direction = 1;
		protected Vector3 _initialPosition;
	    protected Vector3 _finalPosition;
	    protected float _waiting=0;
	    protected int _currentIndex;

		/// <summary>
	    /// Initialization
	    /// </summary>
	    protected virtual void Start ()
		{
			// on Start, we set our active flag to true
			_active=true;

			// if the path is null we exit
			if(Path == null || Path.Count < 1)
			{
				return;
			}

			// we set our initial direction based on the settings
			if (InitialMovementDirection==MovementDirection.Ascending)
			{
				_direction=1;
			}
			else
			{
				_direction=-1;
			}

			// we initialize our path enumerator
			_currentPoint = GetPathEnumerator();
			_currentPoint.MoveNext();

			// initial positioning
			if (!_OriginalTransformPositionSet)
			{
				_OriginalTransformPositionSet=true;
				_OriginalTransformPosition=transform.position;
			}
			transform.position = _OriginalTransformPosition + _currentPoint.Current;
		}

		/// <summary>
		/// On update we keep moving along the path
		/// </summary>
		protected virtual void Update () 
		{
			// if the path is null we exit
			if(Path == null || Path.Count < 1)
			{
				return;
			}

			// we wait until we can proceed
			_waiting-=Time.deltaTime;
			if (_waiting>0)
			{
				CurrentSpeed=Vector3.zero;
				return;
			}

			// we store our initial position to compute the current speed at the end of the udpate	
			_initialPosition=transform.position;

			// we move our object
			MoveAlongThePath();

			// we decide if we've reached our next destination or not, if yes, we move our destination to the next point 
			float distanceSquared = (transform.position - (_OriginalTransformPosition + _currentPoint.Current)).sqrMagnitude;
			if(distanceSquared < MinDistanceToGoal * MinDistanceToGoal)
			{
				//we check if we need to wait
				if (Delays.Count>_currentIndex)
				{
					_waiting=Delays[_currentIndex];				 
				}

				_currentPoint.MoveNext();
			}

			// we determine the current speed		
			_finalPosition=transform.position;
			CurrentSpeed=(_finalPosition-_initialPosition)/Time.deltaTime;
		}

		/// <summary>
		/// Moves the object along the path according to the specified movement type.
		/// </summary>
		public virtual void MoveAlongThePath()
		{
			if(MovementType == PossibleMovementType.ConstantSpeed)
			{
				transform.position = Vector3.MoveTowards(transform.position, _OriginalTransformPosition + _currentPoint.Current, Time.deltaTime * Speed);
				RotateToPoint();
			}

			else if(MovementType== PossibleMovementType.EaseOut)
			{
				transform.position = Vector3.Lerp(transform.position, _OriginalTransformPosition +  _currentPoint.Current, Time.deltaTime * Speed);
			}
		}

		private void RotateToPoint()
		{
			var rot = transform.rotation;
			transform.LookAt(_OriginalTransformPosition + _currentPoint.Current);
			transform.rotation *= Quaternion.Euler(new Vector3(0,-90,0));
			var finalRot = transform.rotation;
			transform.rotation = Quaternion.Lerp(rot, finalRot, Time.deltaTime);
		}

		/// <summary>
		/// Returns the current target point in the path
		/// </summary>
		/// <returns>The path enumerator.</returns>
		public virtual IEnumerator<Vector3> GetPathEnumerator()
		{
			// if the path is null we exit
			if(Path == null || Path.Count < 1)
			{
				yield break;
			}

			int index = 0;
			_currentIndex=index;
			while (true)
			{
				_currentIndex=index;
				yield return Path[index];
				
				if(Path.Count <= 1)
				{
					continue;
				}

				// if the path is looping
				if (LoopPath)
				{
					index = index + _direction;
					if(index < 0)
					{
						index = Path.Count-1;
					}
					else if(index > Path.Count - 1)
					{
						index = 0;
					}
				}
				else
				{
					if(index <= 0)
					{
						_direction = 1;
					}
					else if(index >= Path.Count - 1)
					{
						_direction = -1;
					}
					index = index + _direction;
				}
			}
		}

		/// <summary>
		/// Call this method to force a change in direction at any time
		/// </summary>
		public virtual void ChangeDirection()
		{
			_direction = - _direction;
			_currentPoint.MoveNext();
		}

		/// <summary>
		/// On DrawGizmos, we draw lines to show the path the object will follow
		/// </summary>
		protected virtual void OnDrawGizmos()
		{	
			#if UNITY_EDITOR
			if (Path==null)
			{
				return;
			}

			if (Path.Count==0)
			{
				return;
			}
							
			// if we haven't stored the object's original position yet, we do it
			if (_OriginalTransformPositionSet==false)
			{
		    	_OriginalTransformPosition=transform.position;
				_OriginalTransformPositionSet=true;
			}
			// if we're not in runtime mode and the transform has changed, we update our position
			if (transform.hasChanged && _active==false)
			{
				_OriginalTransformPosition=transform.position;
			}
			// for each point in the path
			for (int i=0;i<Path.Count;i++)
			{
				// we draw a green point 
				Gizmos.color=Color.green;
				Gizmos.DrawSphere(_OriginalTransformPosition+Path[i],0.2f);

				// we draw a line towards the next point in the path
				if ((i+1)<Path.Count)
				{
					Gizmos.color=Color.white;
					Gizmos.DrawLine(_OriginalTransformPosition+Path[i],_OriginalTransformPosition+Path[i+1]);
				}
				// we draw a line from the first to the last point if we're looping
				if ( (i==Path.Count-1) && (LoopPath) )
				{
					Gizmos.color=Color.white;
					Gizmos.DrawLine(_OriginalTransformPosition+Path[0],_OriginalTransformPosition+Path[i]);
				}
			}
			#endif
		}
	}
