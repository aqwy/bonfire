using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Destructible2D
{
	[Serializable] public class D2dDestructibleListEvent : UnityEvent<List<D2dDestructible>> {}
	
	[Serializable] public class D2dCollision2DEvent : UnityEvent<Collision2D> {}
	
	[Serializable] public class D2dFloatFloatEvent : UnityEvent<float, float> {}
	
	[Serializable] public class D2dVector2Event : UnityEvent<Vector2> {}
	
	[Serializable] public class D2dD2dRectEvent : UnityEvent<D2dRect> {}
	
	[Serializable] public class D2dD2dVector2D2dVector2Event : UnityEvent<D2dVector2, D2dVector2> {}
	
	[Serializable] public class D2dEvent : UnityEvent {}
}