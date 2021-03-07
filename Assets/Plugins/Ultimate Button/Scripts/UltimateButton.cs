/* Written by Kaz Crowe */
/* UltimateButton.cs */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/*
 * First off, the script is using [ExecuteInEditMode] to be able to show changes in real time.
 * This will not affect anything within a build or play mode. This simply makes the script
 * able to be run while in the Editor in Edit Mode.
*/
[ExecuteInEditMode]
public class UltimateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	/* ----- > ASSIGNED VARIABLES < ----- */
	RectTransform baseTrans;
	public RectTransform sizeFolder;
	public Image buttonBase;
	public Image buttonHighlight, tensionAccent;
	public Animator buttonAnimator;

	/* ----- > SIZE AND PLACEMENT < ----- */
	public enum Positioning
	{
		Disabled,
		ScreenSpace,
		RelativeToTransform
	}
	public Positioning positioning = Positioning.ScreenSpace;
	public RectTransform relativeTransform;
	public float relativeSpaceMod = 1.5f;

	// SCREEN SPACE
	public enum ScalingAxis{ Width, Height }
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor{ Left, Right }
	public Anchor anchor = Anchor.Right;
	public enum TouchSize
	{
		Default,
		Medium,
		Large
	}
	public TouchSize touchSize = TouchSize.Default;

	public float buttonSize = 1.75f, customSpacing_X = 5.0f, customSpacing_Y = 20.0f;
	CanvasGroup canvasGroup;

	/* ----- > STYLE AND OPTIONS < ----- */
	public enum ImageStyle
	{
		Circular,
		Square
	}
	public ImageStyle imageStyle = ImageStyle.Circular;
	public float inputRange = 1.0f;
	float _inputRange = 1.0f;
	Vector2 buttonCenter = Vector2.zero;
	bool isHovering = false;
	public bool trackInput = false;
	public bool transmitInput = false;
	public GameObject receiver;
	IPointerDownHandler downHandler;
	IDragHandler dragHandler;
	IPointerUpHandler upHandler;
	public enum TapCountOption
	{
		NoCount,
		Accumulate,
		TouchRelease
	}
	public TapCountOption tapCountOption = TapCountOption.NoCount;
	public float tapCountDuration = 0.5f;
	public int targetTapCount = 2;
	float currentTapTime = 0.0f;
	int tapCount = 0;
	public Color baseColor = Color.white;
	public bool showHighlight = false, showTension = false;
	public Color highlightColor = Color.white, tensionColorNone = Color.white, tensionColorFull = Color.white;
	public float tensionFadeInDuration = 1.0f, tensionFadeOutDuration = 1.0f;
	float tensionFadeInSpeed = 1.0f, tensionFadeOutSpeed = 1.0f;
	public bool useAnimation = false, useFade = false;
	public float fadeUntouched = 1.0f, fadeTouched = 0.5f;
	public float fadeInDuration = 1.0f, fadeOutDuration = 1.0f;
	float fadeInSpeed = 1.0f, fadeOutSpeed = 1.0f;

	/* ----- > SCRIPT REFERENCE < ----- */
	public string buttonName;
	static Dictionary<string, UltimateButton> UltimateButtons = new Dictionary<string, UltimateButton>();
	bool getButtonDown = false;
	bool getButton = false;
	bool getButtonUp = false;
	bool getTapCount = false;

	/* ----- > BUTTON EVENTS < ----- */
	public UnityEvent onButtonDown, onButtonUp;
	public UnityEvent tapCountEvent;

	int _pointerId = -10;// -10 is the default value set. -1 and -2 register as mouse ID's.
	

	void Awake ()
	{
		// If the application is being run, then send this button name and states to our static dictionary for reference.
		if( Application.isPlaying == true && buttonName != string.Empty )
		{
			// If the dictionary already contains a Ultimate Button with this name, then remove the button.
			if( UltimateButtons.ContainsKey( buttonName ) )
				UltimateButtons.Remove( buttonName );
			
			// Add the button name and this Ultimate Button into the dictionary.
			UltimateButtons.Add( buttonName, GetComponent<UltimateButton>() );
		}
	}
	
	void Start ()
	{
		// If the application is not running then return.
		if( Application.isPlaying == false )
			return;
		
		// If the user is wanting to show the highlight color of the button, update the highlight images.
		if( showHighlight == true && buttonHighlight != null )
			buttonHighlight.color = highlightColor;

		// If the user is using tension fade options...
		if( showTension == true )
		{
			// Configure the speed variables for the fade.
			tensionFadeInSpeed = 1.0f / tensionFadeInDuration;
			tensionFadeOutSpeed = 1.0f / tensionFadeOutDuration;
		}

		// If the user has useFade enabled...
		if( useFade == true )
		{
			// Get the CanvasGroup component for Enable/Disable options.
			canvasGroup = GetComponent<CanvasGroup>();

			// If the canvasGroup is null, then add a CanvasGroup and get the reference.
			if( canvasGroup == null )
			{
				gameObject.AddComponent( typeof( CanvasGroup ) );
				canvasGroup = GetComponent<CanvasGroup>();
			}

			// Configure the fade speeds.
			fadeInSpeed = 1.0f / fadeInDuration;
			fadeOutSpeed = 1.0f / fadeOutDuration;

			// And apply the default fade for the button.
			canvasGroup.alpha = fadeUntouched;
		}

		// If the parent canvas doesn't have a UltimateButtonUpdater component, then add one.
		Transform parent = transform.parent;
		while( parent != null )
		{
			if( parent.transform.GetComponent<Canvas>() )
			{
				Canvas parentCanvas;
				parentCanvas = parent.transform.GetComponent<Canvas>();
				if( !parentCanvas.GetComponent<UltimateButtonScreenSizeUpdater>() )
					parentCanvas.gameObject.AddComponent( typeof( UltimateButtonScreenSizeUpdater ) );
			}
			parent = parent.transform.parent;
		}

		if( transmitInput == true && receiver != null )
		{
			downHandler = receiver.GetComponent<IPointerDownHandler>();
			dragHandler = receiver.GetComponent<IDragHandler>();
			upHandler = receiver.GetComponent<IPointerUpHandler>();
		}

		// Update the size and positioning of the button.
		UpdatePositioning();
	}
	
	// This function is called when the user has touched down.
	public void OnPointerDown ( PointerEventData touchInfo )
	{
		// If the user wants to transmit the input and the event is assigned, then call the function.
		if( transmitInput == true && downHandler != null )
			downHandler.OnPointerDown( touchInfo );

		// If the button is already in use, then return.
		if( _pointerId != -10 )
			return;

		if( !IsInRange( touchInfo.position ) )
			return;

		_pointerId = touchInfo.pointerId;

		// Set the buttons state to true since it is being interacted with.
		getButton = true;

		// If the button name has been assigned, then broadcast the button's state.
		if( buttonName != string.Empty )
			StartCoroutine( "GetButtonDownDelay" );

		// If the down event is assigned, then call the event.
		if( onButtonDown != null )
			onButtonDown.Invoke();

		// If the user wants to show animations on Touch, set the 'Touch' parameter to true.
		if( useAnimation == true && buttonAnimator != null )
			buttonAnimator.SetBool( "Touch", true );

		// If the user is wanting to count taps on this button...
		if( tapCountOption != TapCountOption.NoCount )
		{
			// If the user is wanting to accumulate taps...
			if( tapCountOption == TapCountOption.Accumulate )
			{
				// If the timer is not currently counting down...
				if( currentTapTime <= 0 )
				{
					// Then start the count down timer, and set the current tapCount to 1.
					StartCoroutine( "TapCountdown" );
					tapCount = 1;
				}
				// Else the timer is already running, so increase tapCount by 1.
				else
					++tapCount;

				// If the timer is still going, and the target tap count has been reached...
				if( currentTapTime > 0 && tapCount >= targetTapCount )
				{
					// Stop the timer by setting the tap time to zero, start the one frame delay for the static reference of tap count, and call the tapCountEvent.
					currentTapTime = 0;
					if( buttonName != string.Empty )
						StartCoroutine( "GetTapCountDelay" );
					
					if( tapCountEvent != null )
						tapCountEvent.Invoke();
				}
			}
			// Else the user is wanting to send tap counts by way of a quick touch and release...
			else
			{
				// If the timer is not currently counting down, then start the coroutine.
				if( currentTapTime <= 0 )
					StartCoroutine( "TapCountdown" );
				else
					currentTapTime = tapCountDuration;
			}
		}

		// If the user wants the button to fade, do that here.
		if( useFade == true && canvasGroup != null )
			StartCoroutine( "ButtonFade" );
		
		// If the user wants to display tension, and the image is assigned, then start the coroutine.
		if( showTension == true && tensionAccent != null )
			StartCoroutine( "TensionAccentFade" );

		// Set is hovering to true since the user has just initiate the touch.
		isHovering = true;
	}

	// This function is called when the user is dragging the input.
	public void OnDrag ( PointerEventData touchInfo )
	{
		// If the user is transmitting input, and the Drag event is assigned, then call the function.
		if( transmitInput == true && dragHandler != null )
			dragHandler.OnDrag( touchInfo );

		// If the pointer event that is calling this function is not the same as the one that initiated the button, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// If the user does not want to track the input when it moves, then return.
		if( trackInput == false )
			return;
		
		if( !IsInRange( touchInfo.position ) && isHovering == true )
		{
			isHovering = false;
			getButton = false;

			if( useAnimation == true && buttonAnimator != null )
				buttonAnimator.SetBool( "Touch", false );
		}
		else if( IsInRange( touchInfo.position ) && isHovering == false )
		{
			isHovering = true;
			getButton = true;

			// If the user is wanting to show tension, start the corresponding coroutine.
			if( showTension == true && tensionAccent != null )
				StartCoroutine( "TensionAccentFade" );

			// If the user is wanting to show fade on the button
			if( useFade == true && canvasGroup != null )
				StartCoroutine( "ButtonFade" );

			if( useAnimation == true && buttonAnimator != null )
				buttonAnimator.SetBool( "Touch", true );
		}
	}
	
	// This function is called when the user has let go of the input.
	public void OnPointerUp ( PointerEventData touchInfo )
	{
		// If the user wants to transmit the input and the OnPointerUp variable is assigned, then call the function.
		if( transmitInput == true && upHandler != null )
			upHandler.OnPointerUp( touchInfo );

		// If the pointer event that is calling this function is not the same as the one that initiated the button, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// Set the buttons state to false.
		getButton = false;

		// Set the stored pointer ID to a null value.
		_pointerId = -10;

		// If the input is not currently hovering over the button, then return.
		if( isHovering == false )
			return;

		// If the button name has been assigned, then broadcast the button's state.
		if( buttonName != string.Empty )
			StartCoroutine( "GetButtonUpDelay" );
		
		// If the up event is assigned, then call the event.
		if( onButtonUp != null )
			onButtonUp.Invoke();

		// If the user is wanting to count the amount of taps by Touch and Release...
		if( tapCountOption == TapCountOption.TouchRelease )
		{
			// Then check the current tap time to see if the release is happening within time.
			if( currentTapTime > 0 )
			{
				// Call the button events.
				if( buttonName != string.Empty )
					StartCoroutine( "GetTapCountDelay" );
					
				if( tapCountEvent != null )
					tapCountEvent.Invoke();
			}

			// Set the tap time to 0 to reset the timer.
			currentTapTime = 0;
		}
		
		// If the user is wanting to show animations, then set the animator.
		if( useAnimation == true && buttonAnimator != null )
			buttonAnimator.SetBool( "Touch", false );

		// Set isHovering to false since the touch input has been released.
		isHovering = false;
	}

	bool IsInRange ( Vector2 inputPos )
	{
		bool inRange = false;
		if( touchSize != TouchSize.Default )
			inRange = true;
		else if( imageStyle == ImageStyle.Circular )
		{
			// Configure the current distance from the center of the button to the touch position.
			float currentDistance = Vector2.Distance( inputPos, buttonCenter );
			if( currentDistance > _inputRange )
				inRange = false;
			else if( currentDistance < _inputRange )
				inRange = true;
		}
		else
		{
			inputPos = inputPos - buttonCenter;
			if( inputPos.x > _inputRange || inputPos.x < -_inputRange || inputPos.y > _inputRange || inputPos.y < -_inputRange )
				inRange = false;
			else
				inRange = true;
		}
		return inRange;
	}

	// This function is used for counting down for the TapCount options.
	IEnumerator TapCountdown ()
	{
		currentTapTime = tapCountDuration;
		while( currentTapTime > 0 )
		{
			currentTapTime -= Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator GetButtonDownDelay ()
	{
		getButtonDown = true;
		yield return new WaitForEndOfFrame();
		getButtonDown = false;
	}

	IEnumerator GetButtonUpDelay ()
	{
		getButtonUp = true;
		yield return new WaitForEndOfFrame();
		getButtonUp = false;
	}

	IEnumerator GetTapCountDelay ()
	{
		getTapCount = true;
		yield return new WaitForEndOfFrame();
		getTapCount = false;
	}

	IEnumerator TensionAccentFade ()
	{
		// Store the current color.
		Color currentColor = tensionAccent.color;

		// If the fade speed is NaN, then just apply the full color.
		if( float.IsInfinity( tensionFadeInSpeed ) )
			tensionAccent.color = tensionColorFull;
		// Else run the loop to fade the tensionAccent.color.
		else
		{
			// For as long as the fade duration, or the button is released...
			for( float fadeIn = 0.0f; fadeIn < 1.0f && getButton == true; fadeIn += Time.deltaTime * tensionFadeInSpeed )
			{
				// Lerp the color between the current color to the full color by the fadeIn value above.
				tensionAccent.color = Color.Lerp( currentColor, tensionColorFull, fadeIn );
				yield return null;
			}
			// If the button is still being interacted with, then apply the final color.
			if( getButton == true )
				tensionAccent.color = tensionColorFull;
		}

		// While the button state is true, yield a frame.
		while( getButton == true )
			yield return null;

		// Re-store the current color.
		currentColor = tensionAccent.color;

		// If the fade speed is NaN, then just apply the None color.
		if( float.IsInfinity( tensionFadeOutSpeed ) )
			tensionAccent.color = tensionColorNone;
		// Else run the loop to fade the tensionAccent.color.
		else
		{
			// For as long as the fade out duration, or in the button gets pressed again...
			for( float fadeOut = 0.0f; fadeOut < 1.0f && getButton == false; fadeOut += Time.deltaTime * tensionFadeOutSpeed )
			{
				// Lerp the tension accent color between the current color to the default color by the fadeOut value above.
				tensionAccent.color = Color.Lerp( currentColor, tensionColorNone, fadeOut );
				yield return null;
			}
			// If the button is still not being interacted with, then apply the final color.
			if( getButton == false )
				tensionAccent.color = tensionColorNone;
		}
	}

	IEnumerator ButtonFade ()
	{
		// Store the current amount of fade.
		float currentFade = canvasGroup.alpha;
		
		// If the fade speed is NaN, then just apply the full color.
		if( float.IsInfinity( fadeInSpeed ) )
			canvasGroup.alpha = fadeTouched;
		else
		{
			for( float fadeIn = 0.0f; fadeIn < 1.0f && getButton == true; fadeIn += Time.unscaledDeltaTime * fadeInSpeed )
			{
				canvasGroup.alpha = Mathf.Lerp( currentFade, fadeTouched, fadeIn );
				yield return null;
			}
			if( getButton == true )
				canvasGroup.alpha = fadeTouched;
		}

		// While the button state is true, yield a frame.
		while( getButton == true )
			yield return null;

		// Store the current fade amount.
		currentFade = canvasGroup.alpha;

		// If the fade speed is NaN, then just apply the untouched color.
		if( float.IsInfinity( fadeOutSpeed ) )
			canvasGroup.alpha = fadeUntouched;
		else
		{
			for( float fadeOut = 0.0f; fadeOut < 1.0f && getButton == false; fadeOut += Time.unscaledDeltaTime * fadeOutSpeed )
			{
				canvasGroup.alpha = Mathf.Lerp( currentFade, fadeUntouched, fadeOut );
			
				yield return null;
			}
			if( getButton == false )
				canvasGroup.alpha = fadeUntouched;
		}
	}

	/// <summary>
	/// Updates the positioning of the Ultimate Button on the screen according to the user's options.
	/// </summary>
	void UpdateSizeAndPlacement ()
	{
		// Find the reference size for the axis to size the button by.
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;

		// Configure a size for the image based on the Canvas's size and scale.
		float textureSize = referenceSize * ( buttonSize / 10 );

		// Then configure position spacers according to canvasSize, the fixed spacing and texture size.
		float positionSpacerX = Screen.width * ( customSpacing_X / 100 ) - ( textureSize * ( customSpacing_X / 100 ) );
		float positionSpacerY = Screen.height * ( customSpacing_Y / 100 ) - ( textureSize * ( customSpacing_Y / 100 ) );

		// Create a temporary Vector2 to modify and return.
		Vector2 imagePosition = new Vector2( anchor == Anchor.Left ? positionSpacerX : ( Screen.width - textureSize ) - positionSpacerX, positionSpacerY );

		// Temporary Vector2 to store the default size of the button.
		Vector2 tempVector = new Vector2( textureSize, textureSize );

		// Apply the button size multiplied by the users touch size option.
		baseTrans.sizeDelta = tempVector * ( touchSize == TouchSize.Large ? 2.0f : touchSize == TouchSize.Medium ? 1.51f : 1.01f );

		// Apply the imagePosition modified with the difference of the sizeDelta divided by 2, multiplied by the scale of the parent canvas.
		baseTrans.position = imagePosition - ( ( baseTrans.sizeDelta - tempVector ) / 2 );

		// Apply the size and position to the sizeFolder.
		sizeFolder.sizeDelta = tempVector;
		sizeFolder.position = imagePosition;

		buttonCenter = sizeFolder.position;
		buttonCenter += new Vector2( baseTrans.sizeDelta.x, baseTrans.sizeDelta.y ) / 2;
		_inputRange = ( ( baseTrans.sizeDelta.x ) / 2 ) * inputRange;

		// If the user wants to fade, and the canvasGroup is unassigned, find the CanvasGroup.
		if( useFade == true && canvasGroup == null )
		{
			if( GetComponent<CanvasGroup>() )
				canvasGroup = GetComponent<CanvasGroup>();
			else
			{
				gameObject.AddComponent<CanvasGroup>();
				canvasGroup = GetComponent<CanvasGroup>();
			}
		}
	}

	/// <summary>
	/// Updates the positioning of the Ultimate Button relative to the target transform.
	/// </summary>
	void UpdateRelativeToTransform ()
	{
		if( relativeTransform == null )
			return;

		// Configure a size for the image based on the Canvas's size and scale.
		float textureSize = relativeTransform.sizeDelta.y * ( this.buttonSize / 5 );

		// First, fix the customSpacing to be a value between 0.0f and 1.0f.
		Vector2 fixedCustomSpacing = new Vector2( customSpacing_X, customSpacing_Y ) / 100;

		// Then configure position spacers according to canvasSize, the fixed spacing and texture size.
		float positionSpacerX = ( relativeTransform.sizeDelta.x * relativeSpaceMod ) * fixedCustomSpacing.x - ( textureSize * fixedCustomSpacing.x );
		float positionSpacerY = ( relativeTransform.sizeDelta.y * relativeSpaceMod ) * fixedCustomSpacing.y - ( textureSize * fixedCustomSpacing.y );

		// Create a temporary Vector2 to modify and return.
		Vector2 tempVector = new Vector2( positionSpacerX, positionSpacerY );

		// Temporary Vector2 to store the default size of the button.
		Vector2 buttonSize = new Vector2( textureSize, textureSize );
		Vector2 refTransPosition = ( ( Vector2 )relativeTransform.position + ( relativeTransform.sizeDelta / 2 ) ) - ( ( relativeTransform.sizeDelta / 2 ) * relativeSpaceMod ) + ( buttonSize / 2 );
		
		// Apply the button size multiplied by the fixedTouchSize.
		baseTrans.sizeDelta = buttonSize;
		baseTrans.position = ( refTransPosition + tempVector ) - ( buttonSize / 2 );

		sizeFolder.sizeDelta = buttonSize;
		sizeFolder.position = baseTrans.position;

		buttonCenter = sizeFolder.position;
		buttonCenter += new Vector2( baseTrans.sizeDelta.x, baseTrans.sizeDelta.y ) / 2;
		_inputRange = ( ( baseTrans.sizeDelta.x ) / 2 ) * inputRange;
	}

	/// <summary>
	/// This function simply waits until the current frame is complete, and then positions the Ultimate Button relative to the target transform. The reason is that if the relative transform is using a script to position itself, then it most likely be positioning itself on Start() as well, which most times will mess up the positioning of the Ultimate Button. By waiting until the next frame it will correctly position itself.
	/// </summary>
	IEnumerator DelayPositioning ()
	{
		yield return new WaitForEndOfFrame();
		UpdateRelativeToTransform();
	}

#if UNITY_EDITOR
	void Update ()
	{
		// The button will be updated constantly when the game is not being run.
		if( Application.isPlaying == false )
			UpdatePositioning();
	}
#endif
	
	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Updates the size and placement of the Ultimate Button. Useful for when applying any options changed at runtime.
	/// </summary>
	public void UpdatePositioning ()
	{
		if( Application.isPlaying )
		{
			// Set the buttons state to false.
			getButton = false;
			getButtonDown = false;
			getButtonUp = false;

			// Set the stored pointer ID to a null value.
			_pointerId = -10;

			// If the user is wanting to show animations, then set the animator.
			if( useAnimation == true && buttonAnimator != null )
				buttonAnimator.SetBool( "Touch", false );

			StopCoroutine( "TensionAccentFade" );
			StopCoroutine( "ButtonFade" );

			if( useFade )
				canvasGroup.alpha = fadeUntouched;
			if( showTension )
				tensionAccent.color = tensionColorNone;

			// Set isHovering to false since the touch input has been released.
			isHovering = false;
		}

		if( positioning == Positioning.Disabled )
			return;

		if( sizeFolder == null )
			return;

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();

		if( positioning == Positioning.ScreenSpace )
			UpdateSizeAndPlacement();
		else
		{
			if( Application.isPlaying )
				StartCoroutine( "DelayPositioning" );
			else
				UpdateRelativeToTransform();
		}
	}

	/// <summary>
	/// Returns true on the frame that the Ultimate Button is pressed down.
	/// </summary>
	public bool GetButtonDown ()
	{
		return getButtonDown;
	}

	/// <summary>
	/// Returns true on the frames that the Ultimate Button is being interacted with.
	/// </summary>
	public bool GetButton ()
	{
		return getButton;
	}

	/// <summary>
	/// Returns true on the frame that the Ultimate Button is released.
	/// </summary>
	public bool GetButtonUp ()
	{
		return getButtonUp;
	}

	/// <summary>
	/// Returns true when the Tap Count option has been achieved.
	/// </summary>
	public bool GetTapCount ()
	{
		return getTapCount;
	}

	/// <summary>
	/// Disables the Ultimate Button.
	/// </summary>
	public void DisableButton ()
	{
		if( !gameObject.activeInHierarchy )
			return;

		// Set the buttons state to false.
		getButton = false;
		getButtonDown = false;
		getButtonUp = false;

		// Set the stored pointer ID to a null value.
		_pointerId = -10;
		
		// If the user is wanting to show animations, then set the animator.
		if( useAnimation == true && buttonAnimator != null )
			buttonAnimator.SetBool( "Touch", false );

		StopCoroutine( "TensionAccentFade" );
		StopCoroutine( "ButtonFade" );

		if( useFade )
			canvasGroup.alpha = fadeUntouched;
		if( showTension )
			tensionAccent.color = tensionColorNone;

		// Set isHovering to false since the touch input has been released.
		isHovering = false;

		// Disable the gameObject.
		gameObject.SetActive( false );
	}

	/// <summary>
	/// Enables the Ultimate Button.
	/// </summary>
	public void EnableButton ()
	{
		if( gameObject.activeInHierarchy )
			return;

		// Enable the gameObject.
		gameObject.SetActive( true );
	}
	/* ------------------------------------------- *** END PUBLIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
	
	/* --------------------------------------------- *** STATIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Returns the targeted Ultimate Button if it exists within the scene.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static UltimateButton GetUltimateButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return null;

		return UltimateButtons[ buttonName ];
	}

	/// <summary>
	/// Returns true on the frame that the targeted Ultimate Button is pressed down.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetButtonDown ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;
		
		return UltimateButtons[ buttonName ].getButtonDown;
	}

	/// <summary>
	/// Returns true on the frames that the targeted Ultimate Button is being interacted with.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getButton;
	}

	/// <summary>
	/// Returns true on the frame that the targeted Ultimate Button is released.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	/// <returns></returns>
	public static bool GetButtonUp ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getButtonUp;
	}

	/// <summary>
	/// Returns true when the Tap Count option has been achieved.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetTapCount ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getTapCount;
	}

	/// <summary>
	/// Disables the targeted Ultimate Button.
	/// </summary>
	/// <param name="buttonName">The name of the desired Ultimate Button.</param>
	public static void DisableButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return;

		UltimateButtons[ buttonName ].DisableButton();
	}

	/// <summary>
	/// Enables the targeted Ultimate Button.
	/// </summary>
	/// <param name="buttonName">The name of the desired Ultimate Button.</param>
	public static void EnableButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return;

		UltimateButtons[ buttonName ].EnableButton();
	}

	static bool ButtonConfirmed ( string buttonName )
	{
		if( !UltimateButtons.ContainsKey( buttonName ) )
		{
			Debug.LogWarning( "No Ultimate Button has been registered with the name: " + buttonName + "." );
			return false;
		}
		return true;
	}
	/* ------------------------------------------- *** END STATIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
}