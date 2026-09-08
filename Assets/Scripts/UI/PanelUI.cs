using UnityEngine;

public class PanelUI : MonoBehaviour
{
    //este script va en cada panel, la idea es que despues desde la clase UIManager se pueda leer el id de cada panel para poder cerrarlo o abrirlo
    public string panelID;

    public virtual void OpenPanel() => gameObject.SetActive(true);
    public virtual void ClosePanel() => gameObject.SetActive(false);
}
