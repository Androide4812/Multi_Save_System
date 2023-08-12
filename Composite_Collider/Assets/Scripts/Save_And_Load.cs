using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Save_And_Load : MonoBehaviour
{
    string save_rute;

    [SerializeField] GameObject _player;

    [SerializeField] Sprite red_ball;
    [SerializeField] Sprite yellow_ball;
    [SerializeField] Sprite green_ball;

    [SerializeField] GameObject coins;
    [SerializeField] GameObject coin_prefab;

    //Al comenzar el juego (antes que cualquier start)
    private void Awake() 
    {
        save_rute = Application.persistentDataPath + "/DatosGuardados.data";

        //si no hay archivo crea uno por defecto
        if ( !File.Exists( save_rute ) )
        {
            mydata reset_data = new mydata();

            reset_data.Fuel = 100f;
            reset_data._color = mydata.Color_type.red;
            //reset_data.position = Vector2.zero;
            reset_data.position[1] = 0f;
            reset_data.position[2] = 0f;
            
            foreach (Transform coin in coins.transform)
                //reset_data.coins.Add(coin.position);
                reset_data.coins.Add( new float[2] {coin.position.x, coin.position.y} );

            write_json( reset_data );
        }
    }

    /////////////////////////////////////////////////////////////
    //                          Jsons                          //
    /////////////////////////////////////////////////////////////
    void write_json(mydata save_me)
    {
        /*string myJson = JsonUtility.ToJson( save_me );
        File.WriteAllText( save_rute, myJson );*/

        SerializeToBinary( save_me );

        //SerializeToBinary_base64( save_me );
    }

    mydata read_json()
    {
        /*string myjson = File.ReadAllText( save_rute );
        return( JsonUtility.FromJson<mydata>( myjson ) );*/

        return( DeserializeToBinary() );

        //return( DeserializeToBinary_base64() );
    }

    /////////////////////////////////////////////////////
    //                 Encryptaciones                  //
    /////////////////////////////////////////////////////
    //Xor cipher
    private string keyWord = "Escribe lo que quieras aqui =D";
    private string EncryptDecrypt( string Data )
    {
        string result = "";

        for (int i = 0; i < Data.Length; i++)
            result += (char) ( Data[i] ^ keyWord[i % keyWord.Length] );
        
        return(result);
    }
    /////////////////////////////////////////////////////

    //Serializacion a binario
    private void SerializeToBinary(mydata data)
    {
        FileStream file_stream = new FileStream(save_rute, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file_stream, data);
        file_stream.Close();
    }
    //Deserializacion a binario
    private mydata DeserializeToBinary()
    {
        FileStream file_stream = new FileStream(save_rute, FileMode.Open);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        mydata data = (mydata) binaryFormatter.Deserialize(file_stream);
        file_stream.Close();
        return(data);
    }
    /////////////////////////////////////////////////////

    //Serializacion a binario_base_64
    private void SerializeToBinary_base64(mydata data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        StreamWriter file = new StreamWriter(save_rute);
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, data);
        string a = System.Convert.ToBase64String(ms.ToArray());
        file.WriteLine(a);
        file.Close();
    }

    //Deserializacion a binario_base_64
    private mydata DeserializeToBinary_base64()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        StreamReader file = new StreamReader(save_rute);
        string a = file.ReadToEnd();
        MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(a));
        mydata data = (mydata) binaryFormatter.Deserialize(ms);
        file.Close();
        return (data);
    }
    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////
    //               Botonos de guardado y cargado             //
    /////////////////////////////////////////////////////////////
    public void save_current_game()
    {
        mydata current_data = new mydata();
        //asigno el combustible actual
        current_data.Fuel = _player.GetComponent<movement>().fuel_time;

        //asigno el color actual
        switch (_player.GetComponentInChildren<SpriteRenderer>().sprite.name)
        {
            case "ball2" : current_data._color = mydata.Color_type.yellow; break;
            case "ball3" : current_data._color = mydata.Color_type.green; break;
            default: current_data._color = mydata.Color_type.red; break;
        }

        //asigno la posicion del player
        //current_data.position = (Vector2) _player.transform.position;
        current_data.position[0] = _player.transform.position.x;
        current_data.position[1] = _player.transform.position.y;

        //asigno los coins disponibles
        foreach (Transform coin in coins.transform)
            //current_data.coins.Add(coin.position);
            current_data.coins.Add( new float[2] {coin.position.x, coin.position.y} );

        write_json( current_data );
    }

    public void load_last_game()
    {
        mydata last_data = read_json();

        //asigno el combustible actual
        _player.GetComponent<movement>().change_fuel( last_data.Fuel );
        
        //asigno el color actual
        SpriteRenderer ball_sprite = _player.GetComponentInChildren<SpriteRenderer>();
        switch (last_data._color)
        {
            case mydata.Color_type.yellow : ball_sprite.sprite = yellow_ball; break;
            case mydata.Color_type.green : ball_sprite.sprite = green_ball; break;
            default: ball_sprite.sprite = red_ball; break;
        }

        //asigno la posicion del player
        //_player.transform.position = (Vector2) last_data.position;
        _player.transform.position = new Vector2( last_data.position[0], last_data.position[1]);

        //asigno los coins disponibles
        foreach (Transform coin in coins.transform)
            Destroy(coin.gameObject);

        foreach (var coin in last_data.coins)
            //Instantiate(coin_prefab, coin, coin_prefab.transform.rotation, coins.transform);
            Instantiate(coin_prefab, new Vector2(coin[0], coin[1]), coin_prefab.transform.rotation, coins.transform);
    }
    /////////////////////////////////////////////////////////////

}

//Clases
[System.Serializable]
public class mydata /*<--- todas las variables que necesitemos guardar*/
{
    public float Fuel = 0;
    public enum Color_type { red, yellow, green};
    public Color_type _color = 0;

    /*public Vector2 position = Vector2.zero;

    public List<Vector2> coins = new List<Vector2>();*/

    public float[] position = new float[2];

    public List<float[]> coins = new List<float[]>();
}
