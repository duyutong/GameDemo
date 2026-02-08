using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using UnityObject = UnityEngine.Object;

public static class ExpandMethod
{
    #region EventTrigger
    /// <summary>
    /// 通过代码添加EventTrigger事件（只能做unity已经定义的事件）
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="eventID"></param>
    /// <param name="call"></param>
    public static void AddTriggerEventListener(this EventTrigger trigger, EventTriggerType eventID, Action<PointerEventData> call)
    {
        TriggerEvent callback = new TriggerEvent();
        callback.AddListener(data => call(data as PointerEventData));
        Entry entry = new Entry() { callback = callback, eventID = eventID };
        trigger.triggers.Add(entry);
    }
    /// <summary>
    /// 通过代码删除EventTrigger事件（只能做unity已经定义的事件）
    /// </summary>
    /// <param name="trigger"></param>
    /// <param name="eventID"></param>
    public static void RemoveEventListener(this EventTrigger trigger, EventTriggerType eventID)
    {
        List<Entry> newTriggers = new List<Entry>();
        foreach (Entry entry in trigger.triggers)
        {
            if (entry.eventID == eventID) continue;
            newTriggers.Add(entry);
        }
        trigger.triggers = newTriggers;
    }
    /// <summary>
    /// 通过代码删除全部EventTrigger事件
    /// </summary>
    /// <param name="trigger"></param>
    public static void RemoveAllEventListener(this EventTrigger trigger)
    {
        trigger.triggers.RemoveAll((item) => { return item != null; });
    }
    #endregion

    #region Transform
    /// <summary>
    /// 设置绝对位置的 x 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">x 坐标值。</param>
    public static void SetPositionX(this Transform transform, float newValue)
    {
        Vector3 v = transform.position;
        v.x = newValue;
        transform.position = v;
    }

    /// <summary>
    /// 设置绝对位置的 y 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">y 坐标值。</param>
    public static void SetPositionY(this Transform transform, float newValue)
    {
        Vector3 v = transform.position;
        v.y = newValue;
        transform.position = v;
    }

    /// <summary>
    /// 设置绝对位置的 z 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">z 坐标值。</param>
    public static void SetPositionZ(this Transform transform, float newValue)
    {
        Vector3 v = transform.position;
        v.z = newValue;
        transform.position = v;
    }

    /// <summary>
    /// 增加绝对位置的 x 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">x 坐标值增量。</param>
    public static void AddPositionX(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.position;
        v.x += deltaValue;
        transform.position = v;
    }

    /// <summary>
    /// 增加绝对位置的 y 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">y 坐标值增量。</param>
    public static void AddPositionY(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.position;
        v.y += deltaValue;
        transform.position = v;
    }

    /// <summary>
    /// 增加绝对位置的 z 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">z 坐标值增量。</param>
    public static void AddPositionZ(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.position;
        v.z += deltaValue;
        transform.position = v;
    }

    /// <summary>
    /// 设置相对位置的 x 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">x 坐标值。</param>
    public static void SetLocalPositionX(this Transform transform, float newValue)
    {
        Vector3 v = transform.localPosition;
        v.x = newValue;
        transform.localPosition = v;
    }

    /// <summary>
    /// 设置相对位置的 y 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">y 坐标值。</param>
    public static void SetLocalPositionY(this Transform transform, float newValue)
    {
        if (transform == null) return;
        Vector3 v = transform.localPosition;
        v.y = newValue;
        transform.localPosition = v;
    }
    public static void SetGameObjectActive(this GameObject gameObject, bool value)
    {
        if (gameObject == null) return;
        gameObject.SetActive(value);
    }

    /// <summary>
    /// 设置相对位置的 z 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">z 坐标值。</param>
    public static void SetLocalPositionZ(this Transform transform, float newValue)
    {
        Vector3 v = transform.localPosition;
        v.z = newValue;
        transform.localPosition = v;
    }

    /// <summary>
    /// 增加相对位置的 x 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">x 坐标值。</param>
    public static void AddLocalPositionX(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localPosition;
        v.x += deltaValue;
        transform.localPosition = v;
    }

    /// <summary>
    /// 增加相对位置的 y 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">y 坐标值。</param>
    public static void AddLocalPositionY(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localPosition;
        v.y += deltaValue;
        transform.localPosition = v;
    }

    /// <summary>
    /// 增加相对位置的 z 坐标。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">z 坐标值。</param>
    public static void AddLocalPositionZ(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localPosition;
        v.z += deltaValue;
        transform.localPosition = v;
    }

    /// <summary>
    /// 设置相对尺寸的 x 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">x 分量值。</param>
    public static void SetLocalScaleX(this Transform transform, float newValue)
    {
        Vector3 v = transform.localScale;
        v.x = newValue;
        transform.localScale = v;
    }

    /// <summary>
    /// 设置相对尺寸的 y 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">y 分量值。</param>
    public static void SetLocalScaleY(this Transform transform, float newValue)
    {
        Vector3 v = transform.localScale;
        v.y = newValue;
        transform.localScale = v;
    }

    /// <summary>
    /// 设置相对尺寸的 z 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="newValue">z 分量值。</param>
    public static void SetLocalScaleZ(this Transform transform, float newValue)
    {
        Vector3 v = transform.localScale;
        v.z = newValue;
        transform.localScale = v;
    }

    /// <summary>
    /// 增加相对尺寸的 x 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">x 分量增量。</param>
    public static void AddLocalScaleX(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localScale;
        v.x += deltaValue;
        transform.localScale = v;
    }

    /// <summary>
    /// 增加相对尺寸的 y 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">y 分量增量。</param>
    public static void AddLocalScaleY(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localScale;
        v.y += deltaValue;
        transform.localScale = v;
    }

    /// <summary>
    /// 增加相对尺寸的 z 分量。
    /// </summary>
    /// <param name="transform"><see cref="Transform" /> 对象。</param>
    /// <param name="deltaValue">z 分量增量。</param>
    public static void AddLocalScaleZ(this Transform transform, float deltaValue)
    {
        Vector3 v = transform.localScale;
        v.z += deltaValue;
        transform.localScale = v;
    }
    /// <summary>
    /// 删除本节点下所有子物体
    /// </summary>
    /// <param name="parent"></param>
    public static void RemoveChildren(this Transform parent)
    {
        if (parent == null) return;
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// 重置Transform的本地尺寸，角度以及位置
    /// </summary>
    /// <param name="value"></param>
    public static void Reset(this Transform value)
    {
        //value.position = Vector3.zero;
        value.localPosition = Vector3.zero;
        value.localRotation = Quaternion.identity;
        value.localScale = Vector3.one;
    }
    /// <summary>
    /// 根据父节点尺寸，等比缩放 RectTransform，
    /// 在不拉伸的前提下，使其完整显示在父节点区域内。
    /// </summary>
    /// <param name="self">
    /// 需要调整尺寸的 RectTransform（保持自身宽高比）
    /// </param>
    /// <param name="parentSize">
    /// 父节点的可用尺寸（通常为父 RectTransform 的 sizeDelta 或 rect.size）
    /// </param>
    public static void AdjustUISize(this RectTransform self, Vector2 parentSize) 
    {
        float desiredAspectRatio = self.rect.width / self.rect.height;
        float currAspectRatio = parentSize.x / parentSize.y;
        Vector2 targetVec;
        if (currAspectRatio > desiredAspectRatio)
        {
            float newWidth = parentSize.y * desiredAspectRatio;
            targetVec = new Vector2(newWidth, parentSize.y);
        }
        else
        {
            float newHeight = parentSize.x / desiredAspectRatio;
            targetVec = new Vector2(parentSize.x, newHeight);
        }
        self.sizeDelta = targetVec;
    }
    /// <summary>
    /// 按照输入的位置参数展示位置，会根据屏幕安全范围做平移
    /// </summary>
    /// <param name="self"></param>
    /// <param name="setPos">世界坐标</param>
    public static void AdjustUIPosition(this RectTransform self, Vector3 setPos, Camera camera)
    {
        Vector2 pivot = self.pivot;
        float rectW = self.rect.width;
        float rectH = self.rect.height;

        Rect screenRect = Screen.safeArea;
        float rootW = screenRect.size.x;
        float rootH = screenRect.size.y;

        Vector2 tempPos = camera.WorldToScreenPoint(setPos);

        Vector2 luPivot = Vector2.up;
        Vector2 rdPivot = Vector2.right;
        Vector2 luPoint = tempPos + new Vector2((luPivot.x - pivot.x) * rectW, (luPivot.y - pivot.y) * rectH);
        Vector2 rdPoint = tempPos + new Vector2((rdPivot.x - pivot.x) * rectW, (rdPivot.y - pivot.y) * rectH);

        Vector2 reviseOffset = Vector2.zero;
        if (luPoint.x < screenRect.x) reviseOffset += new Vector2(screenRect.x - luPoint.x, 0);
        if (luPoint.y > rootH) reviseOffset += new Vector2(0, rootH - luPoint.y);
        if (rdPoint.x > rootW) reviseOffset += new Vector2(rootW - rdPoint.x, 0);
        if (rdPoint.y < screenRect.y) reviseOffset += new Vector2(0, screenRect.y - rdPoint.y);

        tempPos += reviseOffset;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(self.parent.GetComponent<RectTransform>(), tempPos, camera, out localPoint);
        self.localPosition = localPoint;
    }
    public static T GetOrAddComponent<T>(this Transform tran) where T : Component
    {
        return tran.GetComponent<T>() ?? tran.AddComponent<T>();
    }

    public static T AddComponent<T>(this Transform tran) where T : Component
    {
        return tran.gameObject.AddComponent<T>();
    }
    #endregion Transform

    #region Graphic
    /// <summary>
    /// 根据背景框RectTransform自适应Image尺寸
    /// </summary>
    /// <param name="image">要调整的Image</param>
    /// <param name="backgroundRect">背景框RectTransform</param>
    public static void FitImageToRect(this Image image, RectTransform backgroundRect)
    {
        if (image == null || backgroundRect == null || image.sprite == null)
            return;

        image.SetNativeSize();
        RectTransform imageRect = image.rectTransform;

        // 背景框宽高
        float bgWidth = backgroundRect.rect.width;
        float bgHeight = backgroundRect.rect.height;

        // 图片原始宽高
        float imgWidth = image.sprite.rect.width;
        float imgHeight = image.sprite.rect.height;

        // 图片与背景框宽高比
        float bgRatio = bgWidth / bgHeight;
        float imgRatio = imgWidth / imgHeight;

        if (imgRatio > bgRatio)
        {
            // 图片更宽，按宽度适应
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bgWidth);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bgWidth / imgRatio);
        }
        else
        {
            // 图片更高，按高度适应
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bgHeight);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bgHeight * imgRatio);
        }

        // 居中
        imageRect.anchoredPosition = Vector2.zero;
    }
    /// <summary>
    /// 计算一张Texture中有颜色区域的Rect（非透明部分）
    /// </summary>
    /// <param name="texture">传入的Texture2D</param>
    /// <returns>Rect，x/y为偏移量（以Texture中心为原点），width/height为非透明区域大小</returns>
    public static Rect GetColorAreaRect(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return Rect.zero;
        }

        // 获取像素数据
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        int minX = width;
        int maxX = 0;
        int minY = height;
        int maxY = 0;

        bool found = false;

        // 遍历所有像素
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = pixels[y * width + x];
                // 判断透明度阈值，可以自行调整
                if (pixel.a > 0.01f)
                {
                    found = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!found)
        {
            // 图片全透明
            return Rect.zero;
        }

        int rectWidth = maxX - minX + 1;
        int rectHeight = maxY - minY + 1;

        // 计算矩形中心相对于texture中心的偏移
        float texCenterX = width / 2f;
        float texCenterY = height / 2f;
        float rectCenterX = (minX + maxX + 1) / 2f;
        float rectCenterY = (minY + maxY + 1) / 2f;
        float offsetX = rectCenterX - texCenterX;
        float offsetY = rectCenterY - texCenterY;

        // 返回的Rect中：
        // x,y 表示相对于Texture中心的偏移
        // width,height 表示非透明区域大小
        return new Rect(offsetX, offsetY, rectWidth, rectHeight);
    }

    /// <summary>
    /// 把16进制颜色字符串转换为Unity的Color对象
    /// 支持 "#RRGGBB" 和 "#RRGGBBAA"
    /// </summary>
    public static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning($"HexToColor: 无法解析颜色字符串 {hex}，返回白色。");
            return Color.white;
        }
    }

    /// <summary>
    /// 将Texture2D转换成Sprite
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Sprite ToSprite(this Texture2D self)
    {
        Rect rect = new Rect(0, 0, self.width, self.height);
        Vector2 pivot = Vector2.one * 0.5f;
        return Sprite.Create(self, rect, pivot);
    }
    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        Color col = graphic.color;
        graphic.color = new Color(col.r, col.g, col.b, alpha);
    }
    /// <summary>
    /// 设置段前缩进
    /// </summary>
    /// <param name="text">原文本组件</param>
    /// <param name="spaceNum">缩进字符</param>
    public static string SetIndentation(this string str, int spaceNum = 2)
    {
        string spaceItem = "";
        for (int i = 0; i < spaceNum - 1; i++) spaceItem += "&&&";
        string spaceStr = "<color=#FFFFFF00>" + spaceItem + "</color>";
        string newStr = "";
        bool isNeedAddSpace = true;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '\n')
            {
                newStr += str[i];
                isNeedAddSpace = true;
            }
            else if (isNeedAddSpace)
            {
                if (str[i] == ' ')//换行时对于多余的空格需要忽略
                {
                    continue;
                }
                newStr += spaceStr;
                newStr += str[i];
                isNeedAddSpace = false;
            }
            else newStr += str[i];
        }

        return newStr.ToString();

    }
    /// <summary>
    /// 设置文字省略
    /// </summary>
    /// <param name="input">原文本</param>
    /// <param name="trunNum">需要保留的文本长度</param>
    /// <returns></returns>
    public static string SetTruncate(this string input, int trunNum = 5)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.Length <= trunNum) return input;
        else return input.Substring(0, trunNum) + "...";
    }

    /// <summary>
    /// 添加事件前先清空
    /// </summary>
    /// <param name="self"></param>
    /// <param name="action"></param>
    public static void AddListenerBeforeRemoveAll(this Button self, UnityEngine.Events.UnityAction action)
    {
        self.onClick.RemoveAllListeners();
        self.onClick.AddListener(action);
    }
    #endregion

    #region Collection
    /// <summary>
    /// 返回列表中符合条件的item数量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="selectFunc">条件函数，返回一个bool类型判断函数</param>
    /// <returns></returns>
    public static int GetSelectNum<T>(this ICollection<T> ts, Func<T, bool> selectFunc)
    {
        int count = 0;
        if (ts == null) return 0;
        foreach (T t in ts)
        {
            if (selectFunc(t)) count++;
        }
        return count;
    }
    /// <summary>
    /// 返回列表中符合条件的子列表（引用）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="selectFunc">条件函数，返回一个bool类型判断函数</param>
    /// <returns></returns>
    public static List<T> GetSelectCollection<T>(this IList<T> ts, Func<T, bool> selectFunc)
    {
        List<T> result = new List<T>();
        foreach (T t in ts)
        {
            if (selectFunc(t)) result.Add(t);
        }
        return result;
    }
    /// <summary>
    /// 返回列表中符合条件的第一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="selectFunc"></param>
    /// <returns></returns>
    public static T GetSelectItem<T>(this IList<T> ts, Func<T, bool> selectFunc)
    {
        foreach (T t in ts)
        {
            if (selectFunc(t)) return t;
        }
        return default(T);
    }
    /// <summary>
    /// 从列表中移除重复元素，只保留每个元素的第一次出现。
    /// 此方法会修改原列表。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    /// <param name="list">要处理的列表。</param>
    /// <exception cref="ArgumentNullException">当列表为 null 时抛出。</exception>
    public static void RemoveDuplicates<T>(this List<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));

        HashSet<T> seen = new HashSet<T>();
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < list.Count; readIndex++)
        {
            T item = list[readIndex];
            if (seen.Add(item))
            {
                list[writeIndex] = item;
                writeIndex++;
            }
        }

        if (writeIndex < list.Count)
        {
            list.RemoveRange(writeIndex, list.Count - writeIndex);
        }
    }
    public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dic, Func<TValue, bool> func)
    {
        List<TKey> keysToRemove = new List<TKey>();
        foreach (var kvp in dic)
        {
            bool isUnityObj = (kvp.Value as UnityEngine.Object) != null;
            bool isString = kvp.Value is string;

            bool isRemoveType = isUnityObj || isString;
            bool isRemoveCheck = func.Invoke(kvp.Value);
            if (isRemoveType && isRemoveCheck) keysToRemove.Add(kvp.Key);
        }

        // 从字典中删除键
        foreach (var key in keysToRemove)
        {
            TValue value = dic[key];
            bool isUnityObj = (value as UnityEngine.Object) != null;
            bool isString = value is string;
            // 销毁Unity对象
            if (isUnityObj) UnityEngine.Object.Destroy(value as UnityEngine.Object);
            else if (isString) dic[key] = (TValue)(object)string.Empty;

            dic.Remove(key);
        }
    }
    /// <summary>
    /// 获得字典的某一项
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dic"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static KeyValuePair<TKey, TValue> GetPair<TKey, TValue>(this Dictionary<TKey, TValue> dic, int index)
    {
        if (dic != null && dic.Count > index && index >= 0)
        {
            // 将字典的键值对转换为列表，然后获取指定索引的元素
            List<KeyValuePair<TKey, TValue>> keyValueList = new List<KeyValuePair<TKey, TValue>>(dic);

            return keyValueList[index];
        }

        // 如果字典为空或索引无效，返回默认值
        return default(KeyValuePair<TKey, TValue>);
    }
    /// <summary>
    /// 将键和值添加或替换到字典中
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
    {
        if (dic == null) dic = new Dictionary<TKey, TValue>();
        if (dic.ContainsKey(key) == false) dic.Add(key, value);
        else dic[key] = value;
    }

    #endregion

    #region Reflection
    /// <summary>
    /// 对引用类型的深度拷贝
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2">被复制对象类型</typeparam>
    /// <param name="destination"></param>
    /// <param name="source">被复制对象</param>
    /// <param name="isPublicOnly">是否只复制被复制对象的公有变量</param>
    /// <param name="isMatchCase">是否判断变量的大小写</param>
    public static void CopyFrom<T1, T2>(this T1 destination, T2 source, bool isPublicOnly = true, bool isMatchCase = true)
    {
        Type type1 = typeof(T1);
        Type type2 = typeof(T2);

        BindingFlags flags_all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        BindingFlags flags_Public = BindingFlags.Public | BindingFlags.Instance;

        PropertyInfo[] properties1 = type1.GetProperties(isPublicOnly ? flags_Public : flags_all);
        PropertyInfo[] properties2 = type2.GetProperties(isPublicOnly ? flags_Public : flags_all);

        FieldInfo[] fieldInfos1 = type1.GetFields(isPublicOnly ? flags_Public : flags_all);
        FieldInfo[] fieldInfos2 = type2.GetFields(isPublicOnly ? flags_Public : flags_all);


        foreach (FieldInfo field1 in fieldInfos1)
        {
            foreach (FieldInfo field2 in fieldInfos2)
            {
                if (field1.FieldType == typeof(Action<int>)) continue;
                if (field1.FieldType == typeof(Action)) continue;
                bool nameCheck = isMatchCase ? field1.Name.ToLower() == field2.Name.ToLower() : field1.Name == field2.Name;
                if (nameCheck && field1.FieldType == field2.FieldType)
                {
                    if (field2.FieldType.IsValueType || field2.FieldType.Equals(typeof(string)))
                    {
                        field1.SetValue(destination, field2.GetValue(source));
                        break;
                    }
                    else
                    {
                        object retval = Activator.CreateInstance(field1.FieldType);
                        retval.CopyFrom(field2.GetValue(source));
                        field1.SetValue(destination, retval);
                        break;
                    }
                }
            }
        }

        foreach (PropertyInfo prop1 in properties1)
        {

            foreach (PropertyInfo prop2 in properties2)
            {
                if (prop1.Name == prop2.Name && prop1.PropertyType == prop2.PropertyType)
                {
                    if (prop1.PropertyType.IsValueType && prop1.CanWrite)
                    {
                        Debug.LogError(prop1.Name + "  " + prop1.CanWrite);
                        prop1.SetValue(destination, prop2.GetValue(source));
                        break;
                    }
                    else
                    {
                        if (prop1.PropertyType.IsInterface) continue;
                        if (!prop1.CanWrite) continue;
                        object retval = Activator.CreateInstance(prop1.PropertyType);
                        retval.CopyFrom(prop2.GetValue(source));
                        prop1.SetValue(destination, retval);
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 判断对象是否是一个列表
    /// </summary>
    /// <param name="fieldType"></param>
    /// <returns></returns>
    public static bool IsListType(Type fieldType)
    {
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 使用反射为目标对象设置指定字段或属性的值。
    /// </summary>
    /// <param name="target">目标对象，必须是类的实例，不能为 null。</param>
    /// <param name="memberName">字段或属性的名称，区分大小写。</param>
    /// <param name="value">要赋予字段或属性的值，类型将自动转换为目标成员的类型。</param>
    /// <returns>如果设置成功，返回 true；如果找不到对应的字段或属性，或设置失败，返回 false。</returns>
    public static bool SetMemberValue(this object target, string memberName, object value)
    {
        if (target == null || string.IsNullOrEmpty(memberName))
            return false;

        var type = target.GetType();

        // 先尝试设置属性
        PropertyInfo prop = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(target, Convert.ChangeType(value, prop.PropertyType));
            return true;
        }

        // 如果属性没找到，尝试设置字段
        FieldInfo field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, Convert.ChangeType(value, field.FieldType));
            return true;
        }

        // 找不到属性或字段
        return false;
    }
    #endregion

    #region TimeCountdown
    public struct CountdownData
    {
        public int Days;
        public int Hours;
        public int Minutes;
    }
    public static CountdownData CalculateCountdown(long timestampMillis)
    {
        DateTime currentTime = DateTime.Now;
        DateTime targetTime = DateTimeOffset.FromUnixTimeMilliseconds(timestampMillis).LocalDateTime;

        TimeSpan timeLeft = targetTime - currentTime;

        CountdownData countdownData = new CountdownData
        {
            Days = timeLeft.Days,
            Hours = timeLeft.Hours % 24,
            Minutes = timeLeft.Minutes % 60,
        };

        return countdownData;
    }
    public static CountdownData CalculateElapsedTime(long timestampMillis)
    {
        DateTime currentTime = DateTime.Now;
        DateTime startTime = DateTimeOffset.FromUnixTimeMilliseconds(timestampMillis).LocalDateTime;

        TimeSpan timeLeft = currentTime - startTime;

        CountdownData countdownData = new CountdownData
        {
            Days = timeLeft.Days,
            Hours = timeLeft.Hours % 24,
            Minutes = timeLeft.Minutes % 60,
        };

        return countdownData;
    }
    /// <summary>
    /// 时间显示
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string ToTimeFormat(this float time)
    {
        int seconds = (int)time;
        int minutes = seconds / 60;
        seconds %= 60;
        if (minutes < 0) minutes = 0;
        if (seconds < 0) seconds = 0;
        return string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
    public static string ConvertTimestampToLocalTime(this long timestampMs)
    {
        // 毫秒级时间戳 → DateTime
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs);
        DateTime localTime = dateTimeOffset.LocalDateTime;

        // 格式化成 00:00:00 这种形式
        return localTime.ToString("HH:mm:ss");
    }
    #endregion
}
