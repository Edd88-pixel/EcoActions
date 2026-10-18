namespace EcoActions.Core;

public static class FavoriteGestures
{
    public static void Toggle(Account account, string gestureId)
    {
        GestureCatalog.Get(gestureId);
        if (!account.FavoriteGestureIds.Remove(gestureId))
            account.FavoriteGestureIds.Add(gestureId);
    }
}
