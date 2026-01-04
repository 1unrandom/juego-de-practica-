using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playermovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

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

    [Header("Gravedad y Salto")]
    [SerializeField] private float Gravedad = -25f;
    [SerializeField] private float fuerzaSalto = 2.5f;
    [SerializeField] private float saltoDelayConfig = 0f;
    [SerializeField] private float coyoteTimeMax = 0.2f; // <--- TIEMPO DE MARGEN PARA SALTAR EN BAJADAS

    private float coyoteTimeCounter; // Contador interno
    private float tiempoSiguienteSalto = 0f;
    private Vector3 velocidadVertical;

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        estaminaActual = estaminaMax;
    }

    void Update()
    {
        MoverJugador();
        ManejarEstamina();
    }

    private void MoverJugador()
    {
        float ValorHorizontal = Input.GetAxisRaw("Horizontal");
        float ValorVertical = Input.GetAxisRaw("Vertical");

        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift) && estaminaActual > 0 && ValorVertical > 0;
        float velocidadFinal = estaCorriendo ? velocidadcorrer : velocidadMovimiento;

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;
        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionplano = (derechaCamara * ValorHorizontal + adelanteCamara * ValorVertical);
        if (direccionplano.sqrMagnitude > 0.0001f)
            direccionplano.Normalize();

        Vector3 movimientoHorizontal = direccionplano * velocidadFinal;

        // --- LÓGICA DE COYOTE TIME (Para saltar en bajadas) ---
        if (controlador.isGrounded)
        {
            coyoteTimeCounter = coyoteTimeMax; // Resetear el margen si estamos tocando suelo
            if (velocidadVertical.y < 0)
            {
                velocidadVertical.y = -2f; // Fuerza hacia abajo para no "flotar" en rampas
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Empezar a descontar el margen si estamos en el aire
        }

        // --- LÓGICA DE SALTO ---
        // Ahora usamos coyoteTimeCounter > 0 en lugar de controlador.isGrounded
        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f && Time.time >= tiempoSiguienteSalto)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * Gravedad);
            tiempoSiguienteSalto = Time.time + saltoDelayConfig;
            coyoteTimeCounter = 0f; // Gastamos el salto inmediatamente
        }

        velocidadVertical.y += Gravedad * Time.deltaTime;

        // MOVIMIENTO FINAL
        Vector3 movimientoFinal = (movimientoHorizontal + velocidadVertical) * Time.deltaTime;
        controlador.Move(movimientoFinal);

        if (estaCorriendo && direccionplano.magnitude > 0)
        {
            estaminaActual -= consumoPorSegundo * Time.deltaTime;
            tiempoSinCorrer = 0;
        }
    }

    private void ManejarEstamina()
    {
        if (tiempoSinCorrer < delayRecuperacion)
            tiempoSinCorrer += Time.deltaTime;
        else if (estaminaActual < estaminaMax)
            estaminaActual += recuperacionPorSegundo * Time.deltaTime;

        estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMax);

        if (barraEstaminaFill != null)
            barraEstaminaFill.fillAmount = estaminaActual / estaminaMax;
    }
}