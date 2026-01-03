using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{   //permite ajustar la sensibilidad
    public float Sensibilidad = 100;

    public Transform Player;


    float RotacionHorizontal = 0;
    float RotacionVertical = 0;




    
    void Start()
    {
        //bloquea el cursor de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        // oculta el cursos
        Cursor.visible = false;





    }

    
    void Update()
    {   // da los valores del mouse
        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;
        Debug.Log($"MouseEnX={ValorX:F1}, MouseEnY={ValorY:F1}");

        //guardaa los valores y queda el valor para seguir
        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;


        //limita la rotacion de la camara
        RotacionVertical = Math.Clamp(RotacionVertical, -80, 80);

        //hace la rotacion de la camara mas fluida
        transform.localRotation = Quaternion.Euler(RotacionVertical, 0, 0);


        Player.Rotate(Vector3.up * ValorX);
    }
}
