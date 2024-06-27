using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

  public float speed;
  public float laneSpeed;
  public float jumpLength;
  public float jumpHeight;
  public float slideLength;

  private Animator anim;
  private Rigidbody rb;
  private BoxCollider boxCollider;
  private int currentLane = 1;
  private Vector3 verticalTargetPosition;
  private bool isJumping = false;
  private float JumpStartumpStart;
  private bool sliding = false;
  private float slideStart;
  private Vector3 boxColliderSize;
  private bool isSwipping = false;
  private Vector2 startingTouch;

  // Use this for initialization
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    anim = GetComponentInChildren<Animator>();
    boxCollider = GetComponent<BoxCollider>();
    boxColliderSize = boxCollider.size;
    anim.Play("runStart");
  }

  // Update is called once per frame
  void Update()
  {
    //inputs para teclas do teclado
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      ChangeLane(-1);
    }
    else if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      ChangeLane(1);
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      if (!isJumping) // Verifica se não está pulando antes de iniciar o salto
      {
        Jump();
      }
    }
    else if (Input.GetKeyDown(KeyCode.DownArrow)) // Tecla para deslizar
    {
      if (isJumping || !isJumping) // Permite deslizar durante o salto ou no chão
      {
        Slide();
      }
    }
    //input para tela mobile
    if (input.touchCount == 1)
    {
        if (isSwipping)
        {
          Vector2 diff = input.Gettouch(0).position - startingTouch;
          diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
          if(diff.magnitude > 0.01f)
          {
            if(Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
            [
              if(diff.y < 0)
              {
                Slide()
              }
              else
              {
                Jump()
              }
            ]
            else
            {
              if(diff.x < 0)
              {
                ChangeLane(-1);
              }
              else
              {
                ChangeLane(1);
              }
            }
          }
        }

         if (input.Gettouch(0).phase == TouchPhase.Began)
          {
      StartingTouch = input.GetTouch(0).position;
      isSwipping = true;
          }
          else if (input.Gettouch(0).phase == TouchPhase.Ended )
          {
          isSwipping = false;
          }

    }

 

    if (isJumping)
    {
      float ratio = (transform.position.z - jumpStart) / jumpLength;
      if (ratio >= 1f)
      {
        isJumping = false;
        anim.SetBool("Jumping", false);
      }
      else
      {
        verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
      }
    }
    else
    {
      verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
    }

    transform.position = Vector3.MoveTowards(transform.position, new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z), laneSpeed * Time.deltaTime);
  }

  private void FixedUpdate()
  {
    rb.velocity = Vector3.forward * speed;
  }

  void ChangeLane(int direction)
  {
    int targetLane = currentLane + direction;
    if (targetLane < 0 || targetLane > 2)
    {
      return;
    }

    currentLane = targetLane;
    verticalTargetPosition = new Vector3(currentLane - 1, 0, 0);
  }

  void Jump()
  {
    jumpStart = transform.position.z;
    anim.SetFloat("JumpSpeed", speed / jumpLength); // Calcula velocidade de salto
    anim.SetBool("Jumping", true);
    isJumping = true;
  }

  void Slide()
  {
    // Permite deslizar durante o salto
    sliding = true; // Ativa o estado de slide

    slideStart = transform.position.z;
    anim.SetFloat("SlideSpeed", speed / slideLength); // Calcula velocidade de escorregada
    anim.SetBool("Sliding", true);

    // Restaura o tamanho da BoxCollider ao terminar o slide (opcional)
    StartCoroutine(ResetColliderSize());
  }

  IEnumerator ResetColliderSize()
  {
    yield return new WaitForSeconds(slideLength); // Aguarda a duração do slide
    boxCollider.size = boxColliderSize;
    sliding = false;
    anim.SetBool("Sliding", false);
  }
}
