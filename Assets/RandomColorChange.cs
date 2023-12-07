using UnityEngine;
using UnityEngine.UI;

public class RandomColorChange : MonoBehaviour
{
    private Image image;

    void Start()
    {
        // Imageコンポーネントを取得
        image = GetComponent<Image>();

        // 初期の色を設定
        SetRandomColor();

        // 一定の間隔で色を変更するためのInvokeRepeatingを使用
        InvokeRepeating("SetRandomColor", 0.25f, 0.25f);
    }

    void SetRandomColor()
    {
        // ランダムな色を生成
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Imageの色を設定
        image.color = randomColor;
    }
}
