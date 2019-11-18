﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageControl : MonoBehaviour
{
    public GameObject dumi;
    bool m_0touch = true;
    bool m_1touch = false;
    Animation m_Animation;
    GameManagerBit m_GMBit;
    public AnimationClip m_Spin;
    public AnimationClip m_Slide;
    public List<Texture2D> m_ImagesPool = new List<Texture2D>();
    public List<Texture2D> m_ImagesPool2 = new List<Texture2D>();
    public List<Texture2D> m_ImagesPool3 = new List<Texture2D>();
    private List<PalabraBD> palabrasDisponibles = new List<PalabraBD>();
    private int firstImage = 0;

    public List<string> m_PalabrasCastellano = new List<string>();
    public List<string> m_PalabrasCatalan = new List<string>();
    public List<AudioClip> m_AudioPoolCastellano = new List<AudioClip>();
    public List<AudioClip> m_AudioPoolCatalan = new List<AudioClip>();

    public static int m_Length;
    public Image m_Image;
    public Image m_ImageBehind;
    public Text m_Text;
    public List<Font> ourFonts = new List<Font>();
    public AudioSource m_AS;
    public int l_Number;
    void Awake()
    {
        RecolectPalabrasBD();
        m_Length = palabrasDisponibles.Count;
        m_GMBit = GameObject.FindGameObjectWithTag("Bit").GetComponent<GameManagerBit>();
        GameManagerBit.m_Alea = Random.Range(0, m_Length);
    }

    private void RecolectPalabrasBD()
    {
        m_ImagesPool.Clear();
        m_ImagesPool2.Clear();
        m_ImagesPool3.Clear();
        m_PalabrasCastellano.Clear();
        m_PalabrasCatalan.Clear();
        m_AudioPoolCastellano.Clear();
        m_AudioPoolCatalan.Clear();
        palabrasDisponibles.Clear();

        foreach(PalabraBD p in GameManager.palabrasDisponibles)
        {
            /*m_ImagesPool.Add(Resources.Load<Texture2D>("Images/Lite/" + p.image1));
            m_ImagesPool2.Add(Resources.Load<Texture2D>("Images/Lite/" + p.image2));
            m_ImagesPool3.Add(Resources.Load<Texture2D>("Images/Lite/" + p.image3));
            m_PalabrasCastellano.Add(p.nameSpanish);
            m_PalabrasCatalan.Add(p.nameCatalan);
            m_AudioPoolCastellano.Add(p.GetAudioClip(p.audio));
            m_AudioPoolCatalan.Add(p.GetAudioClip(p.audio));*/
            palabrasDisponibles.Add(p);
        }
    }

    void Start()
    {

        if (m_GMBit.repetir)
        {
            l_Number = m_GMBit.numLastImage;
            m_GMBit.repetir = false;
        }
        else
        {
            bool same = true;
            while (same)
            {
                int random = Random.Range(0, m_Length);

                if (random != m_GMBit.numLastImage)
                {
                    GameManagerBit.m_Alea = random;
                    l_Number = GameManagerBit.m_Alea;
                    same = false;
                    m_GMBit.numLastImage = l_Number;
                }
                else
                    Random.InitState(Random.seed + 1);
            }
        }

        m_Animation = GetComponent<Animation>();
        firstImage = Random.Range(0, 3);

        switch (firstImage)
        {
            case 0:
                m_Image.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image1);
                break;
            case 1:
                m_Image.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image2); 
                break;
            case 2:
                m_Image.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image3); 
                break;
            default:
                m_Image.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image1);
                break;
        }

        int otherImage = Random.Range(0, 3);

        while (otherImage == firstImage)
        {
            Random.InitState(Random.seed + 1);
            otherImage = Random.Range(0, 3);
        }

        switch (otherImage)
        {
            case 0:
                m_ImageBehind.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image1);
                break;
            case 1:
                m_ImageBehind.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image2);
                break;
            case 2:
                m_ImageBehind.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image3);
                break;
            default:
                m_ImageBehind.sprite = Resources.Load<Sprite>("Images/Lite/" + palabrasDisponibles[l_Number].image1);
                break;
        }

        m_Text.text = palabrasDisponibles[l_Number].palabraActual;
        SearchFont();
        //m_Text.fontSize = SingletonLenguage.GetInstance().ConvertSizeDependWords(m_Text.text);
        m_AS.clip = palabrasDisponibles[l_Number].GetAudioClip(palabrasDisponibles[l_Number].audio);


    }


    void Update()
    {
        if (GameManager.Instance.InputRecieved() && m_0touch)
        {
            Vector3 positionInput;
            if (Input.touchCount > 0)
                positionInput = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            else
                positionInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((new Vector2(positionInput.x, positionInput.y) - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y)).magnitude < 3f)
            {
                m_Animation.clip = m_Slide;
                m_Animation.Play();
                m_0touch = false;
                m_1touch = true;
            }

        }

        else if (GameManager.Instance.InputRecieved() && m_1touch && !m_Animation.isPlaying && !m_AS.isPlaying)
        {
            Vector3 positionInput;
            if (Input.touchCount > 0)
                positionInput = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            else
                positionInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((new Vector2(positionInput.x, positionInput.y) - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y)).magnitude <= 3f)
            {
                m_Animation.clip = m_Spin;
                m_Animation.Play();
                m_1touch = false;

                StartCoroutine(WaitSeconds(3f));
            }

        }

        IEnumerator WaitSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (GameManager.Instance.Dumi)
            {
                GameObject pinguino = Instantiate(dumi, dumi.transform.position, dumi.transform.rotation);
                pinguino.GetComponent<Dumi>().AudioPositivo();
            }
            m_GMBit.ActivateButtons();
            if (!m_GMBit.repeating)
                m_GMBit.AddCountMiniGameBit();
        }
    }

    private string PutName(int _alea)
    {
        switch (SingletonLenguage.GetInstance().GetLenguage())
        {
            case SingletonLenguage.Lenguage.CASTELLANO:
                return m_PalabrasCastellano[_alea];
            case SingletonLenguage.Lenguage.CATALAN:
                return m_PalabrasCatalan[_alea];
            default:
                return m_PalabrasCastellano[_alea];
        }
    }

    private AudioClip PutAudio(int _alea)
    {
        switch (SingletonLenguage.GetInstance().GetLenguage())
        {
            case SingletonLenguage.Lenguage.CASTELLANO:
                return m_AudioPoolCastellano[_alea];
            case SingletonLenguage.Lenguage.CATALAN:
                return m_AudioPoolCatalan[_alea];
            default:
                return m_AudioPoolCastellano[_alea];

        }
    }


    private void SearchFont()
    {
        switch (SingletonLenguage.GetInstance().GetFont())
        {
            case SingletonLenguage.OurFont.IMPRENTA:
                m_Text.text = m_Text.text.ToLower();
                m_Text.font = ourFonts[0];
                break;
            case SingletonLenguage.OurFont.MANUSCRITA:
                m_Text.text = m_Text.text.ToLower();
                m_Text.font = ourFonts[1];
                break;
            case SingletonLenguage.OurFont.MAYUSCULA:
                m_Text.text = m_Text.text.ToUpper();
                m_Text.font = ourFonts[2];
                break;
            default:
                m_Text.text = m_Text.text.ToLower();
                m_Text.font = ourFonts[0];
                break;
        }
    }

}
