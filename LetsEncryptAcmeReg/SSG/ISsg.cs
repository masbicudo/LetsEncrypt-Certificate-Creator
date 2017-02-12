namespace LetsEncryptAcmeReg.SSG
{
    public interface ISsg
    {
        bool Init(string dir);
        bool InitModel(ISsgController controller, ISsgMasterModel mainModel);
        BindResult? InitControls(IControlCreatorAndBinder createAndBind);

        bool IsValid();
        void Patch();
    }
}