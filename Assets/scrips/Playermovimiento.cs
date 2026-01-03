using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Playermovimiento : MonoBehaviour
{
    //referencias
    [Header("Refencias")]

    [SerializeField] private Transform camara;
    private CharacterController controlador;
// correr
    [Header("Movimiento")]

    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float velocidadcorrer = 8f;
    [SerializeField] private float estaminaMax = 100f;
    [SerializeField] private float consumoPorSegundo = 20f;
    [SerializeField] private float recuperacionPorSegundo = 15f;
    [SerializeField] private float delayRecuperacion = 0.5f;
    [SerializeField] private UnityEngine.UI.Image barraEstaminaFill;
    private float estaminaActual;
    private float tiempoSinCorrer = 0;





    //gravedad
    [Header("Gravedad")]
    [SerializeField] private float Gravedad = -9f;
    private Vector3 velocidadVertical;









    void Start()
    {
        



    }

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();


        if (controlador == null && Camera.main != null)
            camara = Camera.main.transform;

        estaminaActual = estaminaMax;


    }


    void Update()
    {
        MoverJugadorEnPlano();
        AplicarGravedad();
    }
    private void MoverJugadorEnPlano()
     {
        ///--------------------------
        ////CAMINAR CON EL PERSONAJE
        ///--------------------------

        // captura las teclasa (AWSD y flechas)
        float ValorHorizontal = Input.GetAxisRaw("Horizontal");
        float ValorVertical = Input.GetAxisRaw("Vertical");

        //calcula hacia donde mira la camara  solo en eje (Adelante y Atras) y (Derecha y Izquierda)
        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        //Eliminamos eje y por que no lo necesitamos
        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        //Normaliza para no tener valores diferentes
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        //combina las direcciones para tener flechas diagonales
        Vector3 direccionplano = (derechaCamara * ValorHorizontal + adelanteCamara * ValorVertical);

        //evita aumentar la velocidad al caminar en diagonal
        if (direccionplano.sqrMagnitude > 0.0001f)
            direccionplano.Normalize();
        //le da velocidad para dependa del tiempo y no de los FPS
        Vector3 desplazamientoXZ = direccionplano * (velocidadMovimiento * Time.deltaTime);


        controlador.Move(desplazamientoXZ);

        ///------------------------
        ///CORRER CON EL PERSONAJE
        ///------------------------

        bool seEstaMoviendo = direccionplano.sqrMagnitude > 0.0001f;
        bool botonCorrer = Input.GetKey(KeyCode.LeftShift);
        bool puedoCorrer = estaminaActual > 0.01f;
        bool corriendo =botonCorrer && seEstaMoviendo && puedoCorrer;

        if (corriendo)
        {
            estaminaActual -= consumoPorSegundo * Time.deltaTime;
            tiempoSinCorrer = 0f;

        }
        else
        {
            tiempoSinCorrer += Time.deltaTime;
            if (tiempoSinCorrer >= delayRecuperacion)
            {
                estaminaActual += recuperacionPorSegundo * Time.deltaTime;
            }
        }
        estaminaActual = Math.Clamp(estaminaActual, 0f, estaminaMax);
        if(barraEstaminaFill != null)
        {
            barraEstaminaFill.fillAmount = estaminaActual / estaminaMax;
        }

        float velocidadActual = corriendo ? velocidadcorrer : velocidadMovimiento;

        






    }

    private void AplicarGravedad()
    {
        velocidadVertical.y += Gravedad * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);

        if(controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;

        }


    }


}
