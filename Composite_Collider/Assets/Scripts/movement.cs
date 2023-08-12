using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    Rigidbody2D m_Rigidbody;
    public float m_Speed = 5f;

    //fuel
    public float fuel_time {get; private set;} = 100;
    [SerializeField] private Image fuel_bar;

    //player_image
    [SerializeField] private SpriteRenderer ball_img;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Store user input as a movement vector
        Vector3 m_Input = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * m_Speed);
    }

    private void Update() 
    {
        if (fuel_time > 0)
        {
            fuel_time -= Time.deltaTime * 3f;
            fuel_bar.fillAmount = fuel_time / 100f;
        }
    }

    public void charge_fuel()
    {
        fuel_time = 100;
    }

    public void change_color(Sprite color)
    {
        ball_img.sprite = color;
    }

    public void change_fuel(float new_fuel)
    {
        fuel_time = new_fuel;
    }

}
