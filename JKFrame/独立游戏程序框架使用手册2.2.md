# 独立游戏程序框架使用手册

# 独立游戏程序框架使用手册

Verison: V_2.2（2024.3.18）

# 框架启用和功能介绍

​![alt](https://uploadfiles.nowcoder.com/images/20230218/796382749_1676702140412/D2B5CA33BD970F64A6301FA75AE2EB22)​

将框架预制体拖入Hierarchy中即可，脚本中使用时using JKframe命名空间，框架的github地址,为了避免框架中UI部分对Scene场景中交互产生干扰，建议把框架交互屏蔽掉。

​![alt](https://uploadfiles.nowcoder.com/images/20240321/796382749_1711008271604/D2B5CA33BD970F64A6301FA75AE2EB22)​

框架类似工具箱和插件，除了UI窗口外的大多情况下，并不需要继承什么类或接口，直接通过XXXSystem调用即可。主要功能系统：

1. 对象池系统：重复利用GameObject或普通class实例，并且支持设置对象池容量
2. 事件系统：解耦工具，不需要持有引用来进行函数的调用
3. 资源系统

    * Resources版本：关联对象池进行资源的加载卸载
    * Addressables版本：关联对象池进行资源的加载卸载，可结合事件工具做到销毁时自动从Addressables Unload
4. MonoSystem：为不继承MonoBehaviour的对象提供Update、FixedUpdate、协程等功能
5. 音效系统：背景音乐、背景音乐轮播、特效音乐、音量全局控制等
6. 存档系统：

    * 支持多存档
    * 自动缓存，避免频繁读磁盘
    * 存玩家设置类数据，也就是不关联任何一个存档
    * 支持二进制和Json存档，开发时使用Json调试，上线后使用二进制加密与减少文件体积
7. 日志系统：日志控制、保存等
8. UI系统：UI窗口的层级管理、Tips功能
9. 场景系统：对Unity场景加载封装了一层，主要用于监听场景加载进度
10. 本地化系统：分为全局配置和局部配置（随GameObject加载）、UI自动本地化收集器（Text、Image组件无需代码即可自动本地化）

其他功能：

1. 状态机：脚本逻辑状态机。
2. 事件工具： 给物体添加点击、鼠标进入、鼠标拖拽、碰撞、触发、销毁等事件，而不需要额外在该物体上添加脚本等。
3. 协程工具：协程避免GC。

# 对象池

在Unity中，对象的生成、销毁都需要性能开销，在一些特定的应用场景下需要进行大量重复物体的克隆，因此需要通过设计对象池来实现重复利用对象实例，减少触发垃圾回收。常用在频繁创建、销毁对象的情况下，比如子弹、AI生成等等、背包格子。

本框架的对象池系统有两类对象池（GameObject对象池和Object对象池）分别负责对需要在场景中实际激活/隐藏的GameObject和不需要显示在场景里的对象（脚本类、材质资源）进行管理。

本框架提供对象池容量的限制，且初始化时，可以预先传入要放入的对象根据默认容量实例化放入对象池，比如场景中默认使用20发子弹，可以在对象池初始化时就实例化好20枚子弹放入对象池。

如有特殊需求，可以通过持有PoolMoudle层来单独构建一个不同于全局对象池PoolSystem的Pool，默认正常情况下使用全局对象池PoolSystem即可。

## GameObject对象池（GOP）

用于管理实际存在场景中并出现在Hierarchy窗口上的GameObject对象。

### 初始化GOP

```js
// API
//根据keyName初始化GOP
//(string keyName, int maxCapacity = -1,GameObject prefab = null,int defaultQuantity = 0)
PoolSystem.InitGameObjectPool(keyName, maxCapacity, prefab, defaultQuanity);
PoolSystem.InitGameObjectPool(keyName, maxCapacity);
PoolSystem.InitGameObjectPool(keyName);
//根据prefab.name初始化GOP
//	//(GameObject prefab = null, int maxCapacity = -1,GameObject prefab = null,int defaultQuantity = 0)
PoolSystem.InitGameObjectPool(prefab, maxCapacity, defaultQuanity);
PoolSystem.InitGameObjectPool(prefab, maxCapacity);
PoolSystem.InitGameObjectPool(prefab);
//根据GameObject数组大小进行默认容量设置，并将数组对象作为默认对象全部置入对象池
//(string keyName, int maxCapacity = -1, GameObject[] gameObjects = null)
PoolSystem.InitGameObjectPool(keyName, maxCapacity, gameObject);

//简单示例
// 设定一个子弹Bullet对象池，最大容量30,默认填满
Gameobject bullet = GameObject.Find("bullet");
PoolSystem.InitGameObjectPool("Bullet", 30, bullet, 30);
PoolSystem.InitGameObjectPool(bullet, 30, 30);

//最简形式
PoolSystem.InitGameObjectPool(“对象池名字”);
```


* 通过keyName或者直接传入prefab根据prefab.name 指定对象池的名字。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 初始化并不向构建出的空对象池填入内容，但可通过prefab和defaultQuanity设置默认容量填充空对象池（初始化时会自动按默认容量和最大容量的最小值自动生成GameObject放入对象池）。
* 可通过传入GameObject数组初始化对象池的默认容量并放入对象填充空对象池。
* maxCapacity, prefab, defaultQuantity可不填，默认无限容量maxCapacity = -1，不预先放入对象，prefab = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。

### 将对象放入GOP

```js
// API
//根据keyName/obj.name放入对象池
//(string assetName, GameObject obj)
PoolSystem.PushGameObject(keyName, obj);
PoolSystem.PushGameObject(obj);

//简单示例
//将一个子弹对象bullet放入Bullet对象池
PoolSystem.PushGameObject("Bullet", bullet);

// 扩展方法
bullet.GameObjectPushPool();
```


* 通过keyName指定对象池名字放入对象obj，keyName不填则默认对象池名字为obj.name。
* 封装了拓展方法，可以通过对象.GameObjectPushPool()简便地将GameObject放入对象池。


* 可以使用拓展方法直接将对象放入同名对象池内。
* 如果传入的keyName/prefab找不到对应的对象池（未Init），则会直接初始化生成一个同名的，无限容量的对象池并放入本次对象。
* obj为null时本次放入操作无效，会进行报错提示。

### 将对象从GOP中取出

```js
// API
//根据keyName加载GameObject
//(string keyName, Transform parent)
PoolSystem.GetGameObject(keyName, parent);
PoolSystem.GetGameObject(keyName);
//根据keyName和T加载GameObject并获取组件，返回值类型为T
PoolSystem.GetGameObject<T>(keyName, parent);
PoolSystem.GetGameObject<T>(keyName);

//简单实例
//将一个子弹对象从对象池中取出
GameObject bullet = PoolSystem.GetGameObject("Bullet");
//将一个子弹对象从对象池中取出并获取其刚体组件
GameObject bullet = PoolSystem.GetGameOjbect<Rigidbody>("Bullet");

```


* 通过keyName指定对象池名字取出GameObject对象并设置父物体为parent，parent不填则默认无父物体在最顶层。
* 可以通过传参获取对象上的某个组件，组件依托于GameObject存在，因此物体此时也已被从对象池中取出。


* 当某个对象池内无对象时，其对象池仍会被保存，只有通过Clear才能彻底清空对象池。
* 当对象池中无对象仍要取出时，会返回null。

### 清空GOP对象池

```js
//API
//清空（GameObject/Object）对象池
//(bool clearGameObject = true, bool clearCSharpObject = true)
PoolSystem.ClearAll(true, false);
//清空GameObject类对象池中keyName索引的对象池
//(string assetName)
PoolSystem.ClearGameObject(keyName);

//简单实例
//清空所有GOP对象池
PoolSystem.ClearAll(true,false);
//清空Bullet对象池
PoolSystem.ClearGameObject("Bullet");
```


* ClearAll方法用于清空所有GOP/OP对象池，两个bool参数是否清空GOP、是否清空OP。
* 清空某一类GOP通过传入keyName对象池名索引。


* 清空所有对象池时（ClearAll），所有资源都会被释放。
* 清空某一类对象池时,GameObject中的数据载体和根节点会被放回对象池重复利用（使用时无需关心，底层实现）。

## Object对象池（OP）

用于管理脚本类对象等非游戏物体对象，OP的API和GOP类似，只不过在传参部分OP支持更多方式

### 初始化OP

```js
// API
//根据keyName初始化OP
//(string keyName, int maxCapacity = -1, int defaultQuantity = 0)
PoolSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuanity);
PoolSystem.InitObjectPool<T>(keyName, maxCapacity);
PoolSystem.InitObjectPool<T>(keyName);
//根据T的类型名初始化OP
PoolSystem.InitObjectPool<T>(maxCapacity, defaultQuanity);
PoolSystem.InitObjectPool<T>(maxCapacity);
PoolSystem.InitObjectPool<T>();
//根据keyName初始化OP，不考虑默认容量，无需传T
PoolSystem.InitObjectPool(keyName, maxCapacity);
PoolSystem.InitObjectPool(keyName);
//根据type类型名初始化OP
//System.Type type, int maxCapacity = -1
PoolSystem.InitObjectPool(type, maxCapacity);
PoolSystem.InitObjectPool(type);


//简单示例
// 设定一个Data数据类对象池，最大容量30,默认填满
PoolSystem.InitObjectPool<Data>("myData",30,30); //对象池名为myData
PoolSystem.InitObjectPool<Data>(30, 30); //对象池池名为Data
PoolSystem.InitObjectPool(xx.GetType()); //对象池名为xx的类型名
```


* 通过keyName或者直接传入T根据T的类型名指定对象池的名字，优先使用keyName，在没有keyName的情况下以T类型名作为对象池名称。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 可通过T和defaultQuanity设置默认容量（初始化时会自动按默认容量和最大容量的最小值自动生成Object放入对象池），对应GameObject通过prefab和defaultQuanity设置默认容量。
* 泛型T起两个作用，一个是不指定keyName时用于充当type名称，另一个是进行默认容量设置时指定预先放入对象池的对象类型，所以如果不想用默认容量功能可以使用不传T的API。
* maxCapacity, prefab, defaultQuantity可不填，默认无限容量maxCapacity = -1，不预先放入对象，prefab = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。
* OP的初始化和GOP略有不同，使用了泛型T传递类型，参数列表更加精简，但只有有泛型参数的重载方法可以进行默认容量的初始化（需要指定泛型T进行类型转换）。
* 可以选择通过传入某个实例的type类型，初始化同名的无限容量OP。

### 将对象放入OP

```js
// API
//根据keyName/obj.getType().FullName即obj对应的类型名放入对象池
//(object obj, string keyName)
PoolSystem.PushObject(obj, keyName);
PoolSystem.PushObject(obj);

//简单示例
//将一个Data数据类对象data放入Data对象池
PoolSystem.PushObject(data, "Data";
PoolSystem.PushObject(data);

// 扩展方法
bullet.ObjectPushPool();
```


* 通过keyName指定对象池名字放入对象obj，keyName不填则默认对象池名字为obj.name。
* 封装了拓展方法，可以通过对象.GameObjectPushPool()简便地将GameObject放入对象池。
* 可以使用拓展方法直接将对象放入同名对象池内。
* 如果传入的keyName/obj找不到对应的对象池（未Init），则会直接初始化生成一个同名的，无限容量的对象池并放入本次对象。
* obj为null时本次放入操作无效,会进行报错提示。

### 将对象从OP中取出

```js
// API
//根据keyName返回System.object类型对象
//(string keyName)
PoolSystem.GetObject(keyName);
//根据keyName返回T类型的对象
PoolSystem.GetObject<T>(keyName);
//根据T类型名称返回对象
PoolSystem.GetObject<T>();
//根据type类型名返回对象
//(System.Type type)
PoolSystem.GetObject(xx.getType());


//简单实例
//将一个Data数据类对象data从对象池中取出
Data data = PoolSystem.GetObject("Data");
Data data = PoolSystem.GetObject<Data>();
```


* 通过keyName，泛型T，type类型指定对象池名字取出Object对象。
* 优先根据keyName索引，不存在keyName时，则通过泛型T的反射类型和type类型名索引


* 推荐使用泛型方法，否则返回值是object类型还需要手动进行转换。

### 清空OP对象池

```js
//API
//清空（GameObject/Object）对象池
//(bool clearGameObject = true, bool clearCSharpObject = true)
PoolSystem.ClearAll(false, true);
//清空Object类对象池下keyName/T类型名/type类型名对象池
//(string keyName)
PoolSystem.ClearObject(keyName);
PoolSystem.ClearObject<T>();
//(System.Type type)
PoolSystem.ClearObject(xx.getType());

//简单实例
//清空所有OP对象池
PoolSystem.ClearAll(false,true);
//清空Data对象池
PoolSystem.ClearObject("Data");
PoolSystem.ClearObject<Data>();
```


* ClearAll方法用于清空所有GOP/OP对象池，两个bool参数是否清空GOP、是否清空OP。
* 清空某一类OP通过传入keyName/泛型T的反射类型名/type类型名索引。


* 清空所有对象池时（ClearAll），所有资源都会被释放。
* 清空某一类对象池时,Object中的数据载体会被放回对象池重复利用（使用时无需关心，底层实现）。

## 对象池可视化

​![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673620827947/D2B5CA33BD970F64A6301FA75AE2EB22)​

可以通过PoolSystemViewer观察OP和GOP。

​![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673622881184/D2B5CA33BD970F64A6301FA75AE2EB22)​

## 注意

* 对象池的名字可以和放入的对象名字不同，并且每一个放入对象池的对象名词也可以不同（只要类型一致），但为了避免混淆，我们推荐同名（同类名或者同GameObject名）或者使用配置、枚举来记录对象池名。
* PoolSystem可以直接使用，但大多情况下，推荐使用ResSystem来获取GameObject/Object对象来保证返回值不为null。

# 资源系统

资源系统实现了Unity资源、游戏对象、类对象的获取、异步加载，并在加载游戏对象和类对象资源时优先从对象池中获取资源来优化性能，若对象池不存在对应资源再通过资源加载方法进行实例化（因为在直接使用对象池时，返回值允许为null，但）。提供Resources和Addressables两种版本:

* Resources版本，关联对象池进行资源的加载、卸载。
* Addressables版本，除关联对象池进行资源的加载、卸载外，结合事件工具实现对象Destroy时Adressables自动unload。

​![alt](https://uploadfiles.nowcoder.com/images/20230111/796382749_1673420726462/D2B5CA33BD970F64A6301FA75AE2EB22)​

两种版本在框架设置里进行切换，。

## Resources版本

### 普通类对象(obj)

类对象资源不涉及异步加载、Resources和Addressables的区别，直接走对象池系统。

#### 初始化

资源系统的底层基于对象池系统，所以在资源系统层面也开放对对象池的初始化设置，API和PoolSystem一致。

```js
// API
//根据keyName初始化OP
//(string keyName, int maxCapacity = -1, int defaultQuantity = 0)
ResSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuanity);
ResSystem.InitObjectPool<T>(keyName, maxCapacity);
ResSystem.InitObjectPool<T>(keyName);
//根据T的类型名初始化OP
ResSystem.InitObjectPool<T>(maxCapacity, defaultQuanity);
ResSystem.InitObjectPool<T>(maxCapacity);
ResSystem.InitObjectPool<T>();
//根据keyName初始化OP，不考虑默认容量，无需传T
ResSystem.InitObjectPool(keyName, maxCapacity);
ResSystem.InitObjectPool(keyName);
//根据type类型名初始化OP
//System.Type type, int maxCapacity = -1
ResSystem.InitObjectPool(type, maxCapacity);
ResSystem.InitObjectPool(type);

//简单示例
// 设定一个Data数据类对象池，最大容量30,默认填满
ResSystem.InitObjectPool<Data>("myData",30,30);
ResSystem.InitObjectPool<Data>(30, 30);
ResSystem.InitObjectPool(xx.GetType());
```


* 通过keyName或者直接传入T根据T的类型名指定对象池的名字。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 可通过T和defaultQuanity设置默认容量（初始化时会自动按默认容量和最大容量的最小值自动生成T类型的对象放入对象池）。
* 泛型T起两个作用，一个是不指定keyName时用于充当type名称，另一个是进行默认容量设置时指定预先放入对象池的对象类型，所以如果不想用默认容量功能可以使用不传T的API。
* maxCapacity, prefab, defaultQuantity可不填，默认无限容量maxCapacity = -1，不预先放入对象，prefab = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。
* 只有有泛型参数的重载方法可以进行默认容量的初始化（需要指定泛型T进行类型转换）。
* 可以选择通过传入某个实例的type类型，初始化同名的无限容量OP。

#### obj加载

```js
// API
//根据keyName从对象池中获取一个T类型对象，没有则new
//string keyName
ResSystem.GetOrNew<T>(keyName);
//根据T类型名从对象池中获取一个T类型对象，没有则new
ResSystem.GetOrNew<T>();

//简单示例,获取Data数据类的一个对象
GameObject go1 = ResSystem.GetOrNew<Data>("Data");
```


* 通过keyName指定加载的类对象名，不填keyName则按照T的类型名加载。


* 加载时优先通过对象池获取，如果对象池中无对应资源，自动new一个类对象返回，保证返回值不为null,这点体现了资源系统比对象池更完善，对象池get不存在的obj资源返回Null。

#### obj卸载

卸载obj即将obj放回对象池进行资源回收。

```js
//API
//根据keyName/obj类型名将obj放回对象池
//object obj, string keyName
ResSystem.PushObjectInPool(obj);
ResSystem.PushObjectInPool(obj, string keyName);

//简单示例，卸载Data类的对象data
ResSystem.PushObjectInPool(data, "Data");
```


* 通过obj指定卸载的对象，keyName指定对象池名，不填则按照obj的类型名卸载。


* 卸载对象时如果没有初始化过对象池，则对应自动创建一个同名无限量对象池并将obj放入，保证对象卸载成功,这点体现了资源系统比对象池更完善，对象池push未初始化的对象池资源会报错。

### 游戏对象（GameObject）

#### 初始化

资源系统的底层基于对象池系统，所以在资源系统层面也开放对对象池的初始化设置，API和PoolSystem大体一致，在prefab部分传参略有不同，通过传Resources下对应的路径由资源系统获得预制体，并克隆出来放入对象池。

```js
//API
//根据keyName初始化GOP
//(string keyName, int maxCapacity = -1, string assetPath = null, int defaultQuantity = 0)
ResSystem.InitGameObjectPool(keyName, maxCapacity, assetPath, defaultQuantity);
ResSystem.InitGameObjectPool(keyName, maxCapacity);
ResSystem.InitGameObjectPool(keyName);
//根据assetPath切割的资源名初始化GOP
//(string assetPath, int maxCapacity = -1, int defaultQuantity = 0)
ResSystem.InitGameObjectPool(string assetPath, maxCapacity, defaultQuantity);
ResSystem.InitGameObjectPool(string assetPath, maxCapacity);
ResSystem.InitGameObjectPool(string assetPath);


//简单示例
// 设定一个子弹Bullet对象池（假设Bullet的路径在Resources文件夹下），最大容量30,默认填满
Gameobject bullet = GameObject.Find("bullet");
ResSystem.InitGameObjectPool("Bullet", 30, bullet, 30);
ResSystem.InitGameObjectPool(bullet, 30, 30);

//最简形式
ResSystem.InitGameObjectPool(“对象池名字”);
```


* 通过keyName或者直接传入assetPath(完整资源路径)根据切割的资源名指定对象池的名字。
* 传入的assetPath会自动切割获得资源名。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 可通过assetPath获取的资源和defaultQuanity设置默认容量（初始化时会自动按默认容量和最大容量的最小值自动生成GameObject放入对象池）。
* 默认无限容量maxCapacity = -1，不预先放入对象，assetPath = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。
* 注意加载到内存的对象在被实例化之后会被自动释放。

#### GameObject加载并实例化

```js
//API
//加载游戏物体
//(string assetPath, Transform parent = null,string keyName=null)
ResSystem.InstantiateGameObject(assetPath, parent, keyName);
ResSystem.InstantiateGameObject(assetPath, parent);
ResSystem.InstantiateGameObject(assetPath);
ResSystem.InstantiateGameObject(parent, keyName);
//加载游戏物体并获取组件T
ResSystem.InstantiateGameObject<T>(assetPath, parent, keyName);
ResSystem.InstantiateGameObject<T>(assetPath, parent);
ResSystem.InstantiateGameObject<T>(assetPath);
ResSystem.InstantiateGameObject<T>(parent, keyName);
//异步加载(void)游戏物体
//(string path, Action<GameObject> callBack = null, Transform parent = null, string keyName = null)
ResSystem.InstantiateGameObjectAsync(assetPath, Action<GameObject> callBack, parent, keyName);
ResSystem.InstantiateGameObjectAsync(assetPath, Action<GameObject> callBack, parent);
ResSystem.InstantiateGameObjectAsync(assetPath, Action<GameObject> callBack);
ResSystem.InstantiateGameObjectAsync(assetPath);
//异步加载(void)游戏物体并获取组件T
ResSystem.InstantiateGameObjectAsync<T>(assetPath, Action<GameObject> callBack, parent, keyName);
ResSystem.InstantiateGameObjectAsync<T>(assetPath, Action<GameObject> callBack, parent);
ResSystem.InstantiateGameObjectAsync<T>(assetPath, Action<GameObject> callBack);
ResSystem.InstantiateGameObjectAsync<T>(assetPath);

//简单示例
//实例化一个子弹对象（假设Bullet路径在Resources下）
GameObject bullet = ResSystem.InstantiateGameObject("Bullet");
//实例化一个子弹对象取出并获取其刚体组件
Rigidbody rb = ResSystem.InstantiateGameObject<Rigidbody>("Bullet");
//异步实例化一个子弹对象,并在其加载完后坐标归零
void getBullet(GameObject bullet)
{
    bullet.transform.position = Vector3.zero;
    }
ResSystem.InstantiateGameObjectAsync("Bullet", getBullet);
```


* 通过assetPath加载游戏物体并实例化返回。
* 实例化的游戏物体会设置父物体为parent，不填则默认为null无父物体在最顶层。
* 实例化的物体名称优先为keyName，keyName为null时则为assetName。
* 优先根据keyName从对象池获取，不填keyName则根据path加载的资源名在对象池中查找。
* 对象池中找不到根据assetpath走Resources加载出对象，不填assetPath时则通过keyName查询路径加载对象。
* 可以通过传参获取对象上的某个组件，组件依托于GameObject存在，因此物体此时也已被从对象池中取出。
* 异步加载游戏物体及其组件的方法返回值为void类型，无法即时快速加载的游戏物体，需要通过callback回调函数获取加载的GameObject对象并进行使用。


* 资源系统如果资源路径正确，则返回值必不为空，优先从对象池中获取，对象池中不存在则根据Load的对象进行实例化返回。

#### GameObject卸载

卸载GameObject即将GameObject放回对象池进行资源回收。

```js
//API
//根据keyName/gameObject.name回收gameObject
//string keyName, GameObject gameObject
ResSystem.PushGameObjectInPool(string keyName, gameObject);
ResSystem.PushGameObjectInPool(gameObject);


//简单示例，卸载子弹对象bullet
ResSystem.PushGameObjectInPool(bullet, "Bullet");
```


* 通过gameObject指定卸载的对象，keyName指定对象池名，不填则按照gameObject的对象名卸载。


* 卸载对象时如果没有初始化过对象池，则对应自动创建一个同名无限量对象池并将gameObject放入。

### Unity资源

这类资源不需要进行实例化，所以不需要过对象池，只需要直接使用数据或者引用，比如AudioClip，Sprite，prefab。

#### 加载Asset

```js
//API
//根据assetPath异步加载T类型资源
//(string assetPath, Action<T> callBack)
ResSystem.LoadAssetAsync<T>(assetPath, callBack);
//根据assetPath加载T类型资源
ResSystem.LoadAsset<T>(assetPath);
//加载指定路径的所有资源，返回object数组
ResSystem.LoadAssets(assetPath);
//加载指定路径的所有资源返回T类型
ResSystem.LoadAssets<T>(assetPath);

//简单示例，加载Resources下的clip音频资源
ResSystem.LoadAssets<AudioClip>("Resources/clip");
```


* 通过assetPath路径加载资源，T用来指明加载的资源类型。
* 异步加载资源需要通过传入callback回调获取加载的资源并进行使用。
* 加载所有资源时不指定T则返回object数组。
* 注意加载的资源不会被自动释放。

#### 卸载Asset

```js
//API
//卸载某个资源
//（UnityEngine.Object assetToUnload）
ResSystem.UnloadAsset(assetToUnload);
//卸载所有资源
ResSystem.UnloadUnusedAssets();
```

卸载资源实际指释放内存中的asset。

对象池是帮做资源回收利用的，避免频繁GC，对象池管理不了Asset资源。而释放是资源不用了也不需要回收卸载掉就行了，GO的自动释放资源系统已经做好了，Asset需要你根据自己的需求来释放，因为Asset也没有生命周期，只能自己释放。

## Addressables版本

### 普通类对象(obj)

类对象资源不涉及异步加载、Resources和Addressables的区别，直接走对象池系统，。

#### 初始化

资源系统的底层基于对象池系统，所以在资源系统层面也开放对对象池的初始化设置，API和PoolSystem一致。

```js
// API
//根据keyName初始化OP
//(string keyName, int maxCapacity = -1, int defaultQuantity = 0)
ResSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuanity);
ResSystem.InitObjectPool<T>(keyName, maxCapacity);
ResSystem.InitObjectPool<T>(keyName);
//根据T的类型名初始化OP
ResSystem.InitObjectPool<T>(maxCapacity, defaultQuanity);
ResSystem.InitObjectPool<T>(maxCapacity);
ResSystem.InitObjectPool<T>();
//根据keyName初始化OP，不考虑默认容量，无需传T
ResSystem.InitObjectPool(keyName, maxCapacity);
ResSystem.InitObjectPool(keyName);
//根据type类型名初始化OP
//System.Type type, int maxCapacity = -1
ResSystem.InitObjectPool(type, maxCapacity);
ResSystem.InitObjectPool(type);

//简单示例
// 设定一个Data数据类对象池，最大容量30,默认填满
ResSystem.InitObjectPool<Data>("myData",30,30);
ResSystem.InitObjectPool<Data>(30, 30);
ResSystem.InitObjectPool(xx.GetType());
```


* 通过keyName或者直接传入T根据T的类型名指定对象池的名字。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 可通过T和defaultQuanity设置默认容量（初始化时会自动按默认容量和最大容量的最小值自动生成T类型的对象放入对象池）。
* 泛型T起两个作用，一个是不指定keyName时用于充当type名称，另一个是进行默认容量设置时指定预先放入对象池的对象类型，所以如果不想用默认容量功能可以使用不传T的API。
* maxCapacity, prefab, defaultQuantity可不填，默认无限容量maxCapacity = -1，不预先放入对象，prefab = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。
* 只有有泛型参数的重载方法可以进行默认容量的初始化（需要指定泛型T进行类型转换）。
* 可以选择通过传入某个实例的type类型，初始化同名的无限容量OP。

#### obj加载

```js
// API
//根据keyName从对象池中获取一个T类型对象，没有则new
//string keyName
ResSystem.GetOrNew<T>(keyName);
//根据T类型名从对象池中获取一个T类型对象，没有则new
ResSystem.GetOrNew<T>();

//简单示例,获取Data数据类的一个对象
GameObject go1 = ResSystem.GetOrNew<Data>("Data");
```


* 通过keyName指定加载的类对象名，不填keyName则按照T的类型名加载。


* 加载时优先通过对象池获取，如果对象池中无对应资源，自动new一个类对象返回，保证返回值不为null。

#### obj卸载

卸载obj即将obj放回对象池进行资源回收。

```js
//API
//根据keyName/obj类型名将obj放回对象池
//object obj, string keyName
ResSystem.PushObjectInPool(obj);
ResSystem.PushObjectInPool(obj, string keyName);

//简单示例，卸载Data类的对象data
ResSystem.PushObjectInPool(data, "Data");
```


* 通过obj指定卸载的对象，keyName指定对象池名，不填则按照obj的类型名卸载。


* 卸载对象时如果没有初始化过对象池，则对应自动创建一个同名无限量对象池并将obj放入。

### 游戏对象（GameObject）

#### 初始化

资源系统的底层基于对象池系统，所以在资源系统层面也开放对对象池的初始化设置，API和PoolSystem有区别，Addressables版本通过Addressables name来获取prefab（参考副本），Res需要传路径来获取prefab（参考副本）。

```js
//API
//根据keyName初始化GOP
//(string keyName, int maxCapacity = -1, string assetName = null, int defaultQuantity = 0)
ResSystem.InitGameObjectPoolForKeyName(keyName, maxCapacity,assetName, defaultQuantity);
ResSystem.InitGameObjectPoolForKeyName(keyName, maxCapacity);
ResSystem.InitGameObjectPoolForKeyName(keyName);
//根据assetName在Addressables中的资源名初始化GOP
//(string assetName, int maxCapacity = -1, int defaultQuantity = 0)
ResSystem.InitGameObjectPoolForAssetName(assetName, maxCapacity, defaultQuantity);
ResSystem.InitGameObjectPoolForAssetName(assetName, maxCapacity);
ResSystem.InitGameObjectPoolForAssetName(assetName);


//简单示例
// 设定一个子弹Bullet对象池（假设Addressable资源名称为Bullet），最大容量30,默认填满
Gameobject bullet = GameObject.Find("bullet");
ResSystem.InitGameObjectPool("Bullet", 30, bullet, 30);
ResSystem.InitGameObjectPool(bullet, 30, 30);

//最简形式
ResSystem.InitGameObjectPool(“对象池名字”);
```


* 通过keyName或者直接传入assetName（Addressable资源的名称）根据获取的资源名指定对象池的名字，优先keyName，没有keyName则使用assetName。
* 可设置对象池最大容量maxCapacity（超过maxCapacity再放入对象会被Destroy掉）。
* 可通过assetName和defaultQuanity设置默认容量（初始化时会自动按默认容量和最大容量的最小值自动加载GameObject放入对象池）。
* 默认无限容量maxCapacity = -1，不预先放入对象，prefab = null， defaultQuantity = 0。
* defaultQuantity必须小于maxCapacity且如果想使用defaultQuantity则必须填入maxCapacity。


* 可以通过重复初始化一个对象池的maxCapacity实现容量的更改，此时如果重新指定了defaultQuanity，则会补齐差量个数的对象进对象池。

#### GameObject加载并实例化

Addressable版本中游戏物体参数通过Addressable资源名assetName（Res是资源路径assetPath）指定，支持加载出的对象Destroy时在Addressables中自动释放。

```js
//API
//加载游戏物体
//(string assetName, Transform parent = null, string keyName = null, bool autoRelease = true)
ResSystem.InstantiateGameObject(assetName, parent, keyName, autoRelease);
ResSystem.InstantiateGameObject(assetName, parent, keyName);
ResSystem.InstantiateGameObject(assetName, parent);
ResSystem.InstantiateGameObject(assetName);
ResSystem.InstantiateGameObject(parent, keyName, autoRelease);
ResSystem.InstantiateGameObject(parent, keyName);
//加载游戏物体并获取组件T
ResSystem.InstantiateGameObject<T>(assetName, parent, keyName, autoRelease);
ResSystem.InstantiateGameObject<T>(assetName, parent, keyName);
ResSystem.InstantiateGameObject<T>(assetName, parent);
ResSystem.InstantiateGameObject<T>(assetName);
ResSystem.InstantiateGameObject<T>(parent, keyName, autoRelease);
ResSystem.InstantiateGameObject<T>(parent, keyName);
//异步加载(void)游戏物体
//(string assetName, Action<GameObject> callBack = null, Transform parent = null, string keyName = null, bool autoRelease = true)
ResSystem.InstantiateGameObjectAsync(assetName, callBack, parent, keyName, autoRelease);
ResSystem.InstantiateGameObjectAsync(assetName, callBack, parent, keyName);
ResSystem.InstantiateGameObjectAsync(assetName, callBack, parent);
ResSystem.InstantiateGameObjectAsync(assetName, callBack);
ResSystem.InstantiateGameObjectAsync(assetName);
//异步加载(void)游戏物体并获取组件T
//(string assetName, Action<T> callBack = null, Transform parent = null, string keyName = null, bool autoRelease = true)
ResSystem.InstantiateGameObjectAsync<T>(assetName, callBack, parent, keyName, autoRelease);
ResSystem.InstantiateGameObjectAsync<T>(assetName, callBack, parent, keyName);
ResSystem.InstantiateGameObjectAsync<T>(assetName, callBack, parent);
ResSystem.InstantiateGameObjectAsync<T>(assetName, callBack);
ResSystem.InstantiateGameObjectAsync<T>(assetName, callBack);
ResSystem.InstantiateGameObjectAsync<T>(assetName);

//简单示例
//实例化一个子弹对象（假设AB资源名称为Bullet）
GameObject bullet = ResSystem.InstantiateGameObject("Bullet");
//实例化一个子弹对象取出并获取其刚体组件
Rigbody rb = ResSystem.InstantiateGameObject<Rigbody>("Bullet");
//异步实例化一个子弹对象,并在其加载完后坐标归零
void getBullet(GameObject bullet)
{
    bullet.transform.position = Vector3.zero;
    }
ResSystem.InstantiateGameObjectAsync("Bullet", getBullet);
```


* 通过assetName加载游戏物体并实例化返回
* 实例化的游戏物体会设置父物体为parent，不填则默认为null无父物体在最顶层。
* 实例化的物体名称优先为keyName，keyName为null时则为assetName。
* 优先根据keyName从对象池获取，不填keyName则根据assetName在对象池中查找。
* 对象池中无缓存，则根据assetName从Addressable中获取资源,不填assetName则根据keyName从Addressable中获取资源。
* 可以通过传参获取对象上的某个组件，组件依托于GameObject存在，因此物体此时也已被从对象池中取出。
* autoRelease为true则通过事件工具为加载出的对象添加事件监听，会在其Destroy时自动调用Addressables的Release API。
* 异步加载游戏物体及其组件的方法返回值为void类型，无法即时直接加载的游戏物体，需要通过callback回调获取加载的GameObject对象并进行使用。


* 如果资源路径正确，则返回值必不为空，优先从对象池中获取，对象池中不存在则根据Load的对象进行实例化返回。

#### GameObject卸载

卸载GameObject即将GameObject放回对象池进行资源回收。

```js
//API
//根据keyName/gameObject.name卸载gameObject
//(string keyName, GameObject gameObject)
ResSystem.PushGameObjectInPool(keyName, gameObject);
ResSystem.PushGameObjectInPool(gameObject);

//简单示例，卸载子弹对象bullet
ResSystem.PushGameObjectInPool(bullet, "Bullet");
```


* 通过gameObject指定卸载的对象，keyName指定对象池名，不填则按照gameObject的对象名卸载。


* 卸载对象时如果没有初始化过对象池，则对应自动创建一个同名无限量对象池并将gameObject放入。

‍

### Unity资源

这类资源不需要进行实例化，所以不需要过对象池，只需要使用数据或者引用，比如AudioClip，Sprite，prefab(没有经过实例化的GameObject原本)。

#### 加载Asset

```js
//API

//根据assetName加载T类型资源
ResSystem.LoadAsset<T>(assetName);
//根据keyName批量加载所有资源(IList<T>)
//(string keyName, out AsyncOperationHandle<IList<T>> handle, Action<T> callBackOnEveryOne = null)
ResSystem.LoadAssets<T>(keyName, handle, callBackOnEveryOne);

//根据assetName异步加载T类型资源（void）
//(string assetName, Action<T> callBack)
ResSystem.LoadAssetAsync<T>(string assetName, Action<T> callBack);
//根据keyName批量异步加载所有资源（void）
//(string keyName, Action<AsyncOperationHandle<IList<T>>> callBack, Action<T> callBackOnEveryOne = null)
ResSystem.LoadAssetsAsync<T>(keyName, callBack, callBackOnEveryOne);

//简单示例，加载Addressable clip音频资源
ResSystem.LoadAssets<AudioClip>("clip");
```


* 通过path路径加载资源，T用来指明加载的资源类型。
* 异步加载单个资源需要通过传入callback回调获取加载的资源并进行使用。
* 批量加载资源时keyName是Addressable中的Labels。
* handle用于释放资源,批量加载时，如果释放资源要释放掉handle，直接去释放资源是无效的.
* Addressable加载指定keyName的所有资源时，支持每加载一个资源调用一次callBackOnEveryOne。
* 异步加载完指定keyName所有资源时，调用callback获取加载的资源集合并进行使用。
* 注意加载的资源不会被自动释放。

#### 卸载Asset

```js
//API
//释放某个资源
//（T obj）
ResSystem.UnloadAsset<T>(T obj);
//销毁对象并释放资源
//(GameObject obj)
ResSystem.UnloadInstance(obj);
//卸载因为批量加载而产生的handle
//(AsyncOperationHandle<TObject> handle)
UnLoadAssetsHandle<TObject>(handle);
```

卸载Asset即释放资源,可以在Destroy游戏对象的同时释放Addressable资源。

## 资源系统-自动生成资源引用代码

  针对Addressables版本，使用字符串来加载资源方式比较麻烦，而且容易输错，框架提供一种基于引用加载的方式。

​![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673621023967/D2B5CA33BD970F64A6301FA75AE2EB22)​

通过Editor工具会在指定路径下生成资源引用代码R。

  ![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673621057882/D2B5CA33BD970F64A6301FA75AE2EB22)​

```js
// API
//返回一个资源
R.GroupName.AddressableName;
//返回一个资源的实例
//(Transform parent = null,string keyName=null,bool autoRelease = true)
R.GroupName.AddressableName(parent, keyName, autoRelease);
R.GroupName.AddressableName(parent, keyName);
R.GroupName.AddressableName(parent);

//使用示例
//获取一个Bullet预制体资源（不实例化）
Gameobject bullet = R.DefaultLocalGroup.Bullet;
//获取一个Bullet实例
Gameobject bullet = R.DefaultLocalGroup.Bullet(x.transform);

//释放
ResSystem.UnloadAsset<GameObject>(bullet);

```


* R是资源脚本的命名空间，固定。
* GroupName是Addressable的组名。
* AddressableName是资源名。
* 如果填写keyName，则先去对象池中找资源实例，找不着再通过Addressable获取资源并实例化。
* parent为实例的父物体。
* autoRelease为true则实例会在Destroy时自动释放Addressable中对应的资源。

​![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673621142397/D2B5CA33BD970F64A6301FA75AE2EB22)​


对于Sprite的子图，也支持直接引用。

​![alt](https://uploadfiles.nowcoder.com/images/20230113/796382749_1673622257470/D2B5CA33BD970F64A6301FA75AE2EB22)​

```js
//子图
R.LV2.Img_Img_0;
//总图
R.Lv2.Img;
```

# 事件系统

框架的事件系统主要负责高效的方法调用与数据传递，实现各功能之间的解耦，通常在调用某个实例的方法时，必须先获得这个实例的引用或者新实例化一个对象，低耦合度的框架结构希望程序本身不去关注被调用的方法所依托的实例对象是否存在，通过事件系统做中转将功能的调用封装成事件，使用事件监听注册、移除和事件触发完成模块间的功能调用管理。常用在UI事件、跨模块事件上。

事件系统支持无返回值的Action，Func实际应用意义不大。

## 事件监听添加

```js
//API
//添加无参数的事件监听
//string eventName, Action action
EventSystem.AddEventListener(eventName, action); 
//添加多个参数的事件监听
//string eventName
EventSystem.AddEventListener<T>(eventName, action);
EventSystem.AddEventListener<T0, T1>(eventName, action);
EventSystem.AddEventListener<T0, T1, T2>(eventName, action);
...
EventSystem.AddEventListener<T0, T1, ..., T15>(eventName, action);

//简单示例
//添加无参数的事件监听,Doit方法对应名称为Test的事件
EventSystem.AddEventListener("Test", Doit);
void Doit()
{
	Debug.Log("Doit");
}
//添加多个参数的事件监听，Doit2对应名称为TestM的事件，参数为int，string
EventSystem.AddEventListener<int, string>("TestM", Doit2);
void Doit2(int a, string b)
{
	Debug.Log(a);
	Debug.Log(b); 
}
```


* eventName是解耦执行的方法的标记，即事件名，是触发事件时的唯一依据。
* action传无返回值方法。
* T0~T15是泛型，用于指定参数表,支持最多16个参数的action。

## 事件监听移除

```js
//API
//添加无参数的事件监听
//string eventName, Action action
EventSystem.RemoveEventListener(eventName, action); 
//添加多个参数的事件监听
//string eventName
EventSystem.RemoveEventListener<T>(eventName, action);
EventSystem.RemoveEventListener<T0, T1>(eventName, action);
EventSystem.RemoveEventListener<T0, T1, T2>(eventName, action);
...
EventSystem.RemoveEventListener<T0, T1, ..., T15>(eventName, action);

//简单示例
//移除无参数的事件监听,Doit方法对应名称为Test的事件
EventSystem.RemoveEventListener("Test", Doit);
void Doit()
{
	Debug.Log("Doit");
}
//移除多个参数的事件监听，Doit2对应名称为TestM的事件，参数为int，string
EventSystem.RemoveEventListener<int, string>("TestM", Doit2);
void Doit2(int a, string b)
{
	Debug.Log(a);
	Debug.Log(b); 
}
```


* eventName是解耦执行的方法的标记，即事件名，是触发事件时的唯一依据。
* action传无返回值方法。
* T0~T15是泛型，用于指定参数表,支持最多16个参数的action。

## 事件触发

```js
//API
//触发无参数事件
EventSystem.EventTrigger(string eventName);
//触发多个参数事件
EventSystem.EventTrigger<T>(string eventName, T arg);
EventSystem.EventTrigger<T0, T1>(string eventName, T0 arg0, T1 arg1);
EventSystem.EventTrigger<T0, T1,..., T15>(string eventName, T0 arg0, T1 arg1, ..., T15 arg15);
 
//简单示例，使用添加监听的方法例子
EventSystem.EventTrigger("Test");
EventSystem.EventTrigger<int,string>("TestM",1,"test");
```


* eventName是解耦执行的方法的标记，即事件名，是触发事件时的唯一依据。
* T0~T15是泛型，用于指定参数表,支持最多16个参数的action。
* 事件的查询底层使用TryGetValue所以触发不存在的事件并不会报错。

## 事件移除

事件移除和事件监听移除的区别参与：

* 事件监听移除只移除一条Action，比如添加了3次同名事件监听，则移除一次后触发还是会执行两次，且eventName记录不会被移除。
* 事件移除会将事件中心字典中有关eventName的记录连带存储的Action一同清空。

```js
//API
//移除一类事件
//(string eventName)
EventSystem.RemoveEvent(eventName);
//移除事件中心中所有事件
EventSystem.Clear();
```

* eventName是解耦执行的方法的标记，即事件名，是触发事件时的唯一依据。

## 类型事件

支持对参数进行封装为一个struct传递，简化参数列表。

```js
//API
//添加类型事件的监听
//Action<T> action
AddTypeEventListener<T>(action);
//移除类型事件的监听
RemoveTypeEventListener<T>(action);
//移除/删除一个类型事件
RemoveTypeEvent<T>();
//触发类型事件
// (T arg)
TypeEventTrigger<T>(arg);
```


* T是封装的参数列表，一般为struct类型。
* action设计使用封装参数T的事件。
* arg是封装的参数。

## 注意

事件系统的运行逻辑是，预先添加/移除事件监听，再在能够获取相应参数的类内触发事件。

# 音效系统

音效服务集成了背景、特效音乐播放，音量、播放控制功能。包含了全局音量globalVolume、背景音量bgVolume、特效音量effectVolume、静音布尔量isMute、暂停布尔量isPause等音量相关的属性，播放背景音乐的PlayBGAudio方法且，播放特效音乐PlayOnShot方法且重载后支持在指定位置或绑定游戏对象播放特定的音乐，特效音乐由于要重复使用，可以从对象池中获取播放器并自动回收，支持播放后执行回调事件。

## 播放背景音乐

## 音量、播放属性控制

音效服务支持在Inspector面板上的值发生变化时自动执行相应的方法更新音量属性,也可以在属性值变化时自动调用相应的更新方法。

```js
//API & 示例
//全局音量(float,0~1),音量设定为50%
AudioSystem.GlobalVolume = 0.5f;
//背景音乐音量(float,0~1)，音量设定为50%
AudioSystem.BGVolume = 0.5f;
//特效音乐音量(flaot,0~1)
AudioStystem.EffectVolume = 0.5f;
//是否全局静音(bool),true则静音
AudioSystem.IsMute = true;
//背景音乐是否循环,true则循环
AudioSystem.IsLoop = true;
//背景音乐是否暂停，true则暂停
AudioSystem.IsPause = true;
```


* GlobalVolume是全局音量，同时影响背景、特效音乐音量。
* BGVolume是背景音乐音量
* EffectVolume是特效音乐音量。
* IsMute控制全局音量是否静音。
* IsLoop控制背景音乐是否循环。
* IsPause控制背景音乐是否暂停。


支持通过面板更新音量属性。

​![alt](https://uploadfiles.nowcoder.com/images/20230116/796382749_1673859978518/D2B5CA33BD970F64A6301FA75AE2EB22)​

## 播放背景音乐

```js
//API
//播放背景音乐
//(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
AudioSystem.PlayBGAudio(clip, loop, volume, fadeOutTime, fadeInTime);
//轮播多个背景音乐
//(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0);
AudioSystem.PlayBGAudioWithClips(clips, volume, fadeOutTime, fadeInTime);
//停止当前背景音乐
AudioSystem.StopBGAudio();
//暂停当前背景音乐
AudioSystem.PauseBGAudio();
//取消暂停当前音乐
AudioSystem.UnPauseBGAudio();

//简单示例
AudioClip clip = ResSystem.LoadAsset<AudioClip>("music");
AudioSystem.PlayBGAudio(clip);
```


* clip是音乐片段，可以传clip数组来轮播音乐。
* volume是音乐的音量，不指定则按原来的背景音量。
* fadeOutTime是渐出音乐的时间。
* fadeInTime是渐入音乐的时间。
* 停止当前背景音乐会将当前背景音乐置空。
* 暂停音乐可取消暂停恢复。

## 播放特效音乐

```js
//API
//播放一次音效并绑定到游戏物体上，位置随物体变化
//(AudioClip clip, Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
audioSystem.PlayOneShot(clip, component, autoReleaseClip, volumeScale, is3d, callBack);
audioSystem.PlayOneShot(clip, component, autoReleaseClip, volumeScale, is3d);
audioSystem.PlayOneShot(clip, component, autoReleaseClip, volumeScale);
audioSystem.PlayOneShot(clip, component, autoReleaseClip);
audioSystem.PlayOneShot(clip, component);
audioSystem.PlayOneShot(clip);
//在指定位置上播放一次音效
//(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
audioSystem.PlayOneShot(clip, position, autoReleaseClip, volumeScale, is3d, callBack);
audioSystem.PlayOneShot(clip, position, autoReleaseClip, volumeScale, is3d);
audioSystem.PlayOneShot(clip, position, autoReleaseClip, volumeScale);
audioSystem.PlayOneShot(clip, position, autoReleaseClip);
audioSystem.PlayOneShot(clip, position);
//简单示例
//在玩家位置播放一次音效
AudioClip clip = ResSystem.LoadAsset<AudioClip>("music");
audioSystem.PlayOneShot(clip,player.transform.position);
//绑定玩家组件播放一次音效（等同于玩家位置）
audioSystem.PlayOneShot(clip,player.transform);
```


* clip是音乐片段，音效系统中特效音乐在每次播放时优先从对象池中取出挂载了AudioSource的GameObject实例生成并会在音效播放完成后自动回收。
* postion是播放的位置，必填。
* component是绑定的组件，这个API的目的是让音效随着物体移动一起移动，不填则默认不绑定。
* autoReleaseClip代表是否需要在音乐播放结束后自动释放clip资源，Res和Addressable均可。
* volumeScle是音乐的音量，不指定默认按最大音量。
* is3D是启用空间音效，默认开启。
* callBack是回调事件，会在音效播放完执行一个无参无返回值方法。


使用Compoent绑定播放音效时，如果绑定物体如果在播放中被销毁了，那么AudioSource会提前解绑避免一同被销毁（通过事件工具提前添加监听），之后播放完毕会自动回收。

# 存档系统

完成对存档的创建，获取，保存，加载，删除，缓存，支持多存档。存档有两类，一类是用户型存档，存储着某个游戏用户具体的信息，如血量，武器，游戏进度，一类是设置型存档，与任何用户存档都无关，是通用的存储信息，比如屏幕分辨率、音量设置等。

​![alt](https://uploadfiles.nowcoder.com/images/20230119/796382749_1674137857963/D2B5CA33BD970F64A6301FA75AE2EB22)​

存档系统支持两类本地文件：，两者通过框架设置面板进行切换，切换时，原本地文件存档会清空！二进制流文件可读性较差不易修改，Json可读性较强，易修改，存档的数据存在Application.persistentDataPath下。

​![alt](https://uploadfiles.nowcoder.com/images/20230127/796382749_1674819128494/D2B5CA33BD970F64A6301FA75AE2EB22)​

​![alt](https://uploadfiles.nowcoder.com/images/20230127/796382749_1674819145708/D2B5CA33BD970F64A6301FA75AE2EB22)​

SaveData和setting分别存储用户存档和设置型存档。

​![alt](https://uploadfiles.nowcoder.com/images/20230127/796382749_1674819136656/D2B5CA33BD970F64A6301FA75AE2EB22)​

用户存档下根据saveID分成若干文件夹用于存储具体的对象。

## 设置型存档

设置存档实际就是一个全局唯一的存档，可以向其中存储全局通用数据。

### 保存设置

```js
//API
//保存设置到全局存档
//(object saveObject, string fileName)
SaveSystem.SaveSetting(saveObject, fileName);
SaveSystem.SaveSetting(saveObject)
//简单示例
//见下一小节结合加载说明
```


* saveObject是要保存的对象，System.Object类型。
* fileName是保存的文件名称，不填默认取saveObject的类型名。

### 加载设置

```js
///API
//从设置存档中加载设置
// string fileName
SaveSystem.LoadSetting<T>(fileName);
SaveSystem.LoadSetting<T>();

//简单示例
// GameSetting类中存储着游戏名称，作为全局数据
[Serializable]
public class GameSetting
{
    public string gameName;
}
GameSetting gameSetting = new GameSetting();
gameSetting.gameName = "测试";
//保存设置
SaveSystem.SaveSetting(gameSetting);
//取出来用
String gameName = SaveSystem.LoadSetting<gameSetting>().gameName;
```

### 删除设置

```js
//API
//删除用户存档和设置存档
SaveSystem.DeleteAll();
```


* fileName是加载设置存档的文件名，T限定了所存储的数据类型，不填fileName则默认以T的类型名作为文件名加载。

## 用户存档

用户存档与具体的用户相关，不同用户存档位置不同，数据也不同，索引为SaveID。

### 创建用户存档

创建的存档索引默认自增。

```js
//API
SaveSystem.CreateSaveItem();

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
```

### 获取用户存档

#### 存档层面

##### 获取所有用户存档

根据一定规则获取所有用户存档，返回List。

```js
//API
//最新的在最后面
SaveSystem.GetAllSaveItem();
//最近创建的在最前面
SaveSystem.GetAllSaveItemByCreatTime();
//最近更新的在最前面
SaveSystem.GetAllSaveItemByUpdateTime();
//万能解决方案，自定义规则
GetAllSaveItem<T>(Func<SaveItem, T> orderFunc, bool isDescending = false)

//简单示例,万能方案，按照SaveID倒序获得存档
GameSetting gameSetting = new GameSetting();
List<SaveItem> testList = SaveSystem.GetAllSaveItem<int>(oderFunc, true);
//List<SaveItem> testList = SaveSystem.GetAllSaveItem();
foreach (var item in testList)
{
	Debug.Log(item.saveID);
}
//排序依据Func
int oderFunc(SaveItem item)
{
	return item.saveID;
}
```


* 提供多种重载方法获取存档List。
* 支持自定义排序依据的万解决方案，T传比较参数类型，orderFunc传比较方法。

##### 获取某一项用户存档

```js
//API
//(int id, SaveItem saveItem)
SaveSystem.GetSaveItem(id);
SaveSystem.GetSaveItem(saveItem);

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
SaveSystem.GetSaveItem(saveItem);
```


* id是用户存档的编号，存档系统会在创建时指定默认ID，使用时透明，因此推荐使用saveItem传参，saveItem是可维护的。

#### 删除用户存档

##### 删除所有用户存档

```js
//API
//删除所有用户存档
SaveSystem.DeleteAllSaveItem();
```

##### 删除某一项用户存档

```js
//API
//(int id, SaveItem saveItem)
SaveSystem.DeleteSaveItem(id);
SaveSystem.DeleteSaveItem(saveItem);

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
SaveSystem.DeleteSaveItem(saveItem);
```


* id是用户存档的编号，存档系统会在创建时指定默认ID，使用时透明，因此推荐使用saveItem传参，saveItem是可维护的。

### 存档对象层面

#### 保存用户存档中某一对象

```js
//API
//(object saveObject, string saveFileName, SaveItem saveItemint, saveID = 0)
SaveSystem.SaveObject(saveObject, saveFileName, saveID);
SaveSystem.SaveObject(saveObject, saveFileName, saveItem);
SaveSystem.SaveObject(saveObject, saveID);
SaveSystem.SaveObject(saveObject, saveItem);

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
GameSetting gameSetting = new GameSetting();
SaveSystem.SaveObject(gameSetting, saveItem);
```


* saveObject是要保存的对象。
* saveFileName是保存后生成的本地文件名（对象会单独作为一个文件存储在对应saveID的文件夹下），不填则以对象的类型名为文件名。
* saveID/SaveItem是对象存储的存档。
* 保存对象时会更新用户存档缓存。

#### 获取用户存档中某一对象

```js
//API
//(string saveFileName, SaveItem saveItem, int saveID = 0)
SaveSystem.LoadObject<T>(saveFileName, saveID);
SaveSystem.LoadObject<T>(saveFileName, saveItem);
SaveSystem.LoadObject<T>(saveID);
SaveSystem.LoadObject<T>(saveItem);

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
GameSetting gameSetting = new GameSetting();
SaveSystem.SaveObject(gameSetting, saveItem);
GameSetting gameSetting = SaveSystem.LoadObject<GameSetting>(saveItem);

```


* T指定获取对象类型。
* saveFileName是获取对象的文件名，不填则默认以T的类型名作为文件名。
* saveID/SaveItem是对象存储的存档。
* 获取对象优先从缓存中读取，不存在则IO读文件获取，并加入缓存。

#### 删除用户存档中某一对象

```js
//API
//(string saveFileName, SaveItem saveItem, int saveID = 0)
SaveSystem.DeleteObject<T>(saveFileName, saveID);
SaveSystem.DeleteObject<T>(saveFileName, saveItem);
SaveSystem.DeleteObject<T>(saveID);
SaveSystem.DeleteObject<T>(saveItem);

//简单示例
SaveItem saveItem = SaveSystem.CreateSaveItem();
GameSetting gameSetting = new GameSetting();
SaveSystem.DeleteObject(gameSetting, saveItem);
GameSetting gameSetting = SaveSystem.DeleteObject<GameSetting>(saveItem);
```


* T指定获取对象类型。
* saveFileName是获取对象的文件名，不填则默认以T的类型名作为文件名。
* saveID/SaveItem是对象存储的存档。
* 删除某一对象时，如果存在对应的缓存，则一并删除。

#### 注意

在从用户存档中取出对象时，底层优先从缓存中读取，避免读时IO，使用时无需关注。

## 序列化字典，vector，color

框架提供了字典的二进制序列化方法以进行存档，给字典包了一层壳，在序列化和反序列化时自动拆分成List存储、组合成Dictionary使用。同时将Color，vector2，vector3单独封装成结构体进行存储，舍弃掉Unity数据类型中自带的额外方法和属性，只保留rgba和xyz坐标。

```js
//API
Vector3 -> Serialized_Vector3
Vector2 -> Serialized_Vector2
Color -> Serialized_Color
Dictionary->Serialized_Dic
```

在使用时，将原先定义字典等数据的语句关键字进行替换即可，框架重载了赋值运算符，构造函数以及类型转换方法，使得序列化的数据类型可以自动跟原生的Vector2，Vector3，Vector2Int，Vector3Int，Color互转，在使用体验上与原生的关键词无异。

# UI框架

UI框架实现对窗口的生命周期管理，层级遮罩管理，按键物理响应等功能，对外提供窗口的打开、关闭、窗口复用API，对内优化好窗口的缓存、层级问题，能够和场景加载、事件系统联动，将Model、View、Controller完全解耦。通过与配置系统、脚本可视化合作，实现新UI窗口对象的快速开发和已有UI窗口的方便接入。

## 数据结构

虽然本文档使用手册，但为了便于上手理解，简单对UI框架的数据结构进行解释。

```js
	//UI窗口数据字典
	Dictionary<string, UIWindowData> UIWindowDataDic;

	//UI窗口数据类
    public class UIWindowData
    {
        [LabelText("是否需要缓存")] public bool isCache;
        [LabelText("预制体Path或AssetKey")] public string assetPath;
        [LabelText("UI层级")] public int layerNum;
        /// <summary>
        /// 这个元素的窗口对象
        /// </summary>
        [LabelText("窗口实例")] public UI_WindowBase instance;

        public UIWindowData(bool isCache, string assetPath, int layerNum)
        {
            this.isCache = isCache;
            this.assetPath = assetPath;
            this.layerNum = layerNum;
            instance = null;
        }
    }
```

UI框架的核心在于维护字典UIWindowDataDic，通过windowKey索引了不同的UI窗口数据UiWindowData，其中包含了窗口是否要缓存，资源路径，UI层级，以及窗口类实例（脚本作为窗口对象的组件，持有他就相当于持有了窗口gameObject），UIWindowData可以通过运行时动态加载也可以在Editor时通过特性静态加载，设计windowKey的原因是如果不额外标定windowKey直接用资源路径作为索引，则同一个窗口资源无法复用，换句话说，同一个UI窗口游戏对象及窗口类，通过不同的windowKey和实例可以进行重用。

## UI窗口对象及类配置

使用UI框架需要先为UI窗口游戏对象添加控制类，该类继承自UI_WindowBase,并将UI窗口游戏对象加入Addressable列表/Resources文件夹下。

## UI窗口特性-Editor静态加载

可以选择为UI窗口类打上UIWindowData特性（Attribute可省略）用于配置数据。  
​![alt](https://uploadfiles.nowcoder.com/images/20230208/796382749_1675823761787/D2B5CA33BD970F64A6301FA75AE2EB22)​

```js
UIWindowDataAttribute(string windowKey, bool isCache, string assetPath, int layerNum){}
UIWindowDataAttribute(Type type,bool isCache, string assetPath, int layerNum){}
```


* 特性中windowKey是UI窗口的名字唯一索引，可以直接传string也可以传Type使用其FullName。
* isCache指明UI窗口游戏对象是否需要缓存重用，true则在窗口关闭时不会被销毁，下次使用时可以通过windowKey调用且不需要实例化。
* assetPath是资源的路径，在Resources中是UI窗口对象在Resources文件夹下的路径，Addressable中是UI窗口对象的Addressable Name。
* layerNum是UI窗口对象的层级，从0开始，越大则越接近顶层。
* 支持一个窗口类多特性，复用同一份窗口类资源，n个特性，则有n份UI窗口数据，本质上对应了多个windowKey，因此windowKey必须不同。

经过配置后，在Editor模式下该UI类特性数据及UI窗口游戏对象（此时还没有实例化为空）会自动保存到GameRoot的配置文件中，即静态加载。

## UI窗口运行时动态加载

在运行时动态加载UI窗口，不需要给窗口类打特性，窗口数据直接给出，与Onshow/OnClose不同，其不包含窗口游戏物体对象的显示/隐藏/销毁逻辑。

```js
//API
//(string windowKey, UIWindowData windowData, bool instantiateAtOnce = false)
UISystem.AddUIWindowData(windowKey, windowData, instantiateAtOnce);
UISystem.AddUIWindowData(windowKey, windowData, instantiateAtOnce);
//(Type type, UIWindowData windowData, bool instantiateAtOnce = false)
UISystem.AddUIWindowData(type, windowData, instantiateAtOnce);
UISystem.AddUIWindowData(type, windowData);
UISystem.AddUIWindowData<T>(windowData, instantiateAtOnce);
UISystem.AddUIWindowData<T>(windowData);

//简单实例
UISystem.AddUIWindowData("Test1", new UIWindowData(true, "TestWindow", 1));
//上一步只添加了数据，显示在面板上还需要激活
UISystem.Show<TestWindow>("Test1");
```


* 通过泛型T指定UI窗口子类类型，windowKey为UI窗口类的索引，对应UIWindowData中的windowKey，不指定则使用T的类型名作为索引。
* instantiateAtOnce指明窗口对象及其类是否要进行实例化，默认为null，会在窗口打开时加载资源进行实例化且设置为不激活，若窗口资源较大，可以提前在动态加载时就进行实例化，如图。

​![alt](https://uploadfiles.nowcoder.com/images/20230217/796382749_1676626104632/D2B5CA33BD970F64A6301FA75AE2EB22)​

## UI窗口数据管理

获取UI窗口数据，其中包含UI的windowKey，层级，资源路径，以及对象实例，可以对其进行操作。

```js
//获取UI窗口数据
//(string windowKey) (Type windowType)
UISystem.GetUIWindowData(windowKey);
UISystem.GetUIWindowData<T>();
UISystem.GetUIWindowData(windowType);
//尝试获取UI窗口数据，返回bool
//(string windowKey, out UIWindowData windowData
UISystem.TryGetUIWindowData(windowKey, windowData);
//移除某条UI窗口数据
//(string windowKey, bool destoryWidnow = false)
UISystem.RemoveUIWindowData(windowKey, destoryWidnow);
//清除所有UI窗口数据
UISystem.ClearUIWindowData();

//简单实例
//获取testWindow的层级
UISystem.GetUIWindowData<testWindow>().layerNum;
```


* 通过windowKey/泛型类型名/窗口对象类型传索引。
* 支持Try方式获取窗口数据，成功返回true并将数据赋给输出参数。
* 移除UI窗口数据,已存在的窗口对象实例会被强行删除。

## UI窗口对象管理

这里的UI窗口对象只UI窗口数据UIWIndowData持有的那一份窗口脚本对象实例，其生命周期由框架管理，整体分为打开和关闭。

### UI窗口打开

加载UI窗口对象并显示。

```js
//API
//返回值为UI窗口类T，T受泛型约束必须为UI窗口基类子类
//(string windowKey, int layer = -1)
UISystem.Show<T>(windowKey, layer);
UISystem.Show<T>(windowKey);
UISystem.Show<T>(layer);
UISystem.Show<T>();
//返回值为UI_WindowBase类，对应不能确定窗口类型的情况, xx是窗口类的对象
//(Type type, int layer = -1)
UISystem.Show(xx.getType(), layer);
UISystem.Show(xx.getType());
//(string windowKey, int layer = -1)
UISystem.Show(windowKey, layer);
UISystem.Show(windowKey);

//简单实例，打开窗口UI_WindowTest并置于第三层
UISystem.Show<UI_WindowTest>(2);
```


* 通过泛型T指定UI窗口子类类型，windowKey为UI窗口类的索引，对应UIWindowData中的windowKey，不指定则使用T的类型名作为索引，layer代表UI的层级，不填则默认-1表示使用数据中原有的层级（通过静态配置或者动态加载指定）。
* 在明确UI窗口类型的时候可以直接通过泛型T指定，不明确则可以通过传对象反射来获取类型。
* 简单解释逻辑为根据windowKey找到对应的窗口数据UIWindowData，根据数据中的assetPath加载UI窗口对象并根据T返回窗口类，无T则返回UI_WindowBase类。

由于UI窗口类继承了UIWIndowBase，其中提供了一些可供重写的方法，这些方***在UI窗口打开时自动执行。

```js
	//初始化相关方法，只有在窗口第一次打开时执行
    public override void Init()
    {
        base.Init();
    }

	//窗口每次打开时执行，可用于数初始化，并会自动调用事件监听注册方法
    public override void OnShow()
    {
        base.OnShow();
    }
	//事件监听注册
    protected override void RegisterEventListener()
    {
        base.RegisterEventListener();
    }
```

### UI窗口关闭

```js
//API
//(Type type) (string windowKey)
UISystem.Close<T>();
UISystem.Close(type);
UISystem.Close(windowKey);
UISystem.TryClose(windowKey);
UISystem.CloseAllWindow();

//简单实例，关闭窗口UI_WindowTest
UISystem.Close<UI_WindowTest>();
```


* 相比打开，关闭不需要返回值也不需要管理层级，通过T/Type/windowKey传入窗口的索引即可。
* * TryClose API在遇到窗口已关闭或不存在时并不会warning，而其他API会报warning。

由于UI窗口类继承了UIWIndowBase，其中提供了一些可供重写的方法，这些方法在UI窗口关闭时自动执行。

```js
	//窗口每次关闭时执行，会动调用事件监听注销方法
    public override void OnClose()
    {
        base.OnClose();
    }

	//事件监听注销
    protected override void RegisterEventListener()
    {
        base.RegisterEventListener();
    }
```

### 获取/销毁UI窗口对象

获取/销毁UIWindowData持有的UI窗口对象实例，与Onshow/OnClose不同，其只获取实例，不包含窗口游戏物体对象的显示/隐藏/销毁逻辑。

```js
//API
//返回值为UI窗口类T，T受泛型约束必须为UI窗口基类子类
//(string windowKey)
UISystem.GetWindow<T>(windowKey);
UISystem.GetWindow<T>(Type windowType);
UISystem.GetWindow<T>();
//返回值为UI_WindowBase类，对应不能确定窗口类型的情况
UISystem.GetWindow(windowKey);
//返回值为bool,表示窗口对是否存在
//(string windowKey, out T window)
UISystem.TryGetWindow(windowKey, window);
//(string windowKey, out T window)
UISystem.TryGetWindow<T>(windowKey, window);
//销毁窗口对象
UISystem.DestroyWindow(windowKey);

//简单实例，获取TestWindow上的UI Text组件Name
Text name = UISystem.GetWindow<TestWindow>().Name;
```


* 通过windowKey/type Name/T类型名查找窗口对象。
* 支持Try方式，查询成功则对象传递到输出参数out上，并返回bool为true，否则输出参数为null并返回false。
* 销毁窗口对象API会直接销毁游戏内的窗口gameObject、控制类，但UIWindowData还存在。

## UI层级管理

框架内部实现了对UI的层级管理，可以在面板的UISystem上每一层是否启用遮罩，默认每一层UI是层层堆叠覆盖的，一旦某一层中有UI窗口对象，则层级比它低的层级都不可以交互，同一层级中比它早打开的UI窗口不可以交互（保证每一层内最顶层只有一个窗口），可以勾选不启用遮罩，则这一层层内和层外都不存在遮罩关系。

启用遮罩如下图。

​![alt](https://uploadfiles.nowcoder.com/images/20230217/796382749_1676624683901/D2B5CA33BD970F64A6301FA75AE2EB22)​

Mask保证了每一层内最顶层只有一个窗口进行交互。

​![alt](https://uploadfiles.nowcoder.com/images/20230217/796382749_1676628517227/D2B5CA33BD970F64A6301FA75AE2EB22)​

另外框架单独提供了最顶层dragLayer，用于拖拽时临时需要把某个UI窗口置于最上层，可以通过UISystem.dragLayer获取。

```js
UISystem.dragLayer;
```

## UI Tips

弹窗工具。

```js
//API
// 在窗口右下角弹出字符串tips提醒。
//(string tips)
UISystem.AddTips(tips)
```

## 判断鼠标是否在UI上

返回当前鼠标位置是否在UI上，（用于替换EventSystem.current.IsPointerOverGameObject()，避免当前窗口因启用交互或同时需要考虑多层UI的层级关系，而启用覆盖全屏幕的遮罩Mask的RaycastTaret，使得鼠标处于UI窗口外时，Unity API一直错误的返回在UI上）。

```js
//bool
UISystem.CheckMouseOnUI();
```

# 日志系统

日志系统用于在控制台输出Log、Success、Error、Warning的提示信息（用白色、绿色、红色、黄色加以区分），并可以进行本地自定义命名保存，可以在面板上勾选是否启用日志输出、写入时间（毫秒级定位）、线程ID、堆栈(定位提示代码行)、本地保存。

​![alt](https://uploadfiles.nowcoder.com/images/20230211/796382749_1676097857332/D2B5CA33BD970F64A6301FA75AE2EB22)​

保留Unity提示自带的代码连接跳转功能。

​![alt](https://uploadfiles.nowcoder.com/images/20230211/796382749_1676097916169/D2B5CA33BD970F64A6301FA75AE2EB22)​

本地保存的日志可以用于在打包后进行调试输出。

​![alt](https://uploadfiles.nowcoder.com/images/20230211/796382749_1676098150424/D2B5CA33BD970F64A6301FA75AE2EB22)​

```js
//API
//输出日志测试信息，等同于Debug.Log
JKLog.Log("测试Log");
//输出Warning类型的提示信息
JKLog.Warning("测试Warning");
//输出Error类型的提示信息
JKLog.Error("测试Error");
//输出Succeed类型的输出信息
JKLog.Succeed("测试Succeed");
```


* 在方法参数部分传入要输出的字符串信息即可。

# 事件工具

用于给游戏对象快速绑定事件，而无需手动给游戏对象挂载脚本，功能逻辑在当前脚本实现。与事件系统区分：事件系统重点在于提供了一个事件监听添加和事件触发解耦的中间模块，使得事件的触发无需关注依赖的对象，但事件执行的功能逻辑还是要实现在对象挂载的脚本上的。而事件工具重点在于快速为游戏对象绑定常见的响应事件，这类事件不由脚本触发（后续支持自定义脚本触发条件），而是在特定的时机如碰撞、鼠标点击、对象销毁时自动触发，因此重点关注事件监听添加的简化，所有逻辑在当前脚本完成。

## 框架内置事件绑定与移除

### 鼠标相关事件

鼠标进入、鼠标移出、鼠标点击、鼠标按下、鼠标抬起、鼠标拖拽、鼠标拖拽开始、鼠标拖拽结束事件的绑定与移除。

```js
//鼠标进入
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnMouseEnter<TEventArg>(action, args);
xx.OnMouseEnter(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnMouseEnter(action); //无参Action
xx.RemoveOnMouseEnter<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标移出
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnMouseExit<TEventArg>(action, args);
xx.OnMouseExit(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnMouseExit(action); //无参Action
xx.RemoveOnMouseExit<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标点击
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnClick<TEventArg>(action, args);
xx.OnClick(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnClick(action); //无参Action
xx.RemoveOnClick<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标按下
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnClickDown<TEventArg>(action, args);
xx.OnClickDown(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnClickDown(action); //无参Action
xx.RemoveOnClickDown<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标抬起
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnClickUp<TEventArg>(action, args);
xx.OnClickUp(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnClickUp(action); //无参Action
xx.RemoveOnClickUp<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标拖拽
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnDrag<TEventArg>(action, args);
xx.OnDrag(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnDrag(action); //无参Action
xx.RemoveOnDrag<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标拖拽开始
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnBeginDrag<TEventArg>(action, args);
xx.OnBeginDrag(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnBeginDrag(action); //无参Action
xx.RemoveOnBeginDrag<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//鼠标拖拽结束
//(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnEndDrag<TEventArg>(action, args);
xx.OnEndDrag(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnEndDrag(action); //无参Action
xx.RemoveOnEndDrag<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//使用示例
Transform cube;
void Start()
{
	cube.OnClick<int>(Test1,1);
}
private void Test1(PointerEventData arg1, int arg2)
{
	Debug.Log(1);
    cube.RemoveOnClick<int>(Test1);
}
```


* xx为绑定事件的对象组件，事件工具基于拓展方法调用，xx使用游戏对象的transform即可。
* TEventArg指定事件的参数类型，添加监听时可以不填，可以通过参数args推断出，移除监听时则必须显示指出。
* action是绑定的事件，根据事件类型（鼠标、碰撞、自定义事件），其方法的参数列表包含两部分，第一部分是事件本身的参数（PointerEventData、Collision），第二部分是参数列表TEventArg，可以通过值元组传入多个参数。

### 碰撞相关事件

2D、3D相关的碰撞事件绑定与移除。

```js
//API
//3D碰撞进入
//(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionEnter<TEventArg>(action, args);
xx.OnCollisionEnter(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionEnter(action); //无参Action
xx.RemoveOnCollisionEnter<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//3D碰撞持续
//(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionStay<TEventArg>(action, args);
xx.OnCollisionStay(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionStay(action); //无参Action
xx.RemoveOnCollisionStay<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//3D碰撞脱离
//(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionExit<TEventArg>(action, args);
xx.OnCollisionExit(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionExit(action); //无参Action
xx.RemoveOnCollisionExit<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D碰撞进入
//(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionEnter2D<TEventArg>(action, args);
xx.OnCollisionEnter2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionEnter2D(action); //无参Action
xx.RemoveOnCollisionEnter2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D碰撞持续
//(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionStay2D<TEventArg>(action, args);
xx.OnCollisionStay2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionStay2D(action); //无参Action
xx.RemoveOnCollisionStay2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D碰撞脱离
//(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnCollisionExit2D<TEventArg>(action, args);
xx.OnCollisionExit2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnCollisionExit2D(action); //无参Action
xx.RemoveOnCollisionExit2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//简单示例
void Start()
{
	cube.OnCollisionEnter(Test2, 2);
}

private void Test2(Collision arg1, int arg2)
{
	Debug.Log(arg2);
	cube.RemoveOnCollisionEnter<int>(Test2);
}
```


* 碰撞事件和鼠标事件的API类似，区别参与action的第一个事件本身参数不同，为Collision/Collision2D。
* xx为绑定事件的对象组件，使用游戏对象的transform即可。
* TEventArg指定事件的参数类型，添加监听时可以不填，可以通过参数args推断出，移除监听时则必须显示指出。
* action是绑定的事件，根据事件类型（鼠标、碰撞、自定义事件），其方法的参数列表包含两部分，第一部分是事件本身的参数（PointerEventData、Collision），第二部分是参数列表TEventArg，可以通过值元组传入多个参数。

### 触发相关事件

2D、3D相关的触发事件绑定。

```js
//API
//3D触发进入
//(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerEnter<TEventArg>(action, args);
xx.OnTriggerEnter(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerEnter(action); //无参Action
xx.RemoveOnTriggerEnter<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//3D触发持续
//(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerStay<TEventArg>(action, args);
xx.OnTriggerStay(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerStay(action); //无参Action
xx.RemoveOnTriggerStay<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//3D触发脱离
//(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerExit<TEventArg>(action, args);
xx.OnTriggerExit(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerExit(action); //无参Action
xx.RemoveOnTriggerExit<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D触发进入
//(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerEnter2D<TEventArg>(action, args);
xx.OnTriggerEnter2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerEnter2D(action); //无参Action
xx.RemoveOnTriggerEnter2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D碰撞持续
//(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerStay2D<TEventArg>(action, args);
xx.OnTriggerStay2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerStay2D(action); //无参Action
xx.RemoveOnTriggerStay2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//2D触发脱离
//(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnTriggerExit2D<TEventArg>(action, args);
xx.OnTriggerExit2D(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnTriggerExit2D(action); //无参Action
xx.RemoveOnTriggerExit2D<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//简单示例

void Start()
{
	cube.OnTriggerEnter(Test3, 2);
}

private void Test3(Collider arg1, int arg3)
{
	Debug.Log(arg3);
	cube.RemoveOnTriggerEnter<int>(Test3);
}
```


* 触发事件和碰撞事件的API类似，区别参与action的第一个事件本身参数不同，为Collider/Collider2D。
* xx为绑定事件的对象组件，使用游戏对象的transform即可。
* TEventArg指定事件的参数类型，添加监听时可以不填，可以通过参数args推断出，移除监听时则必须显示指出。
* action是绑定的事件，根据事件类型（鼠标、碰撞、自定义事件），其方法的参数列表包含两部分，第一部分是事件本身的参数（PointerEventData、Collision），第二部分是参数列表TEventArg，可以通过值元组传入多个参数。

### 资源相关事件

资源释放，对象销毁时绑定的事件。

```js
//API
//资源释放（Addressable）
//(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnReleaseAddressableAsset<TEventArg>(action, args);
xx.OnReleaseAddressableAsset(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnReleaseAddressableAssetOnReleaseAddressableAsset(action); //无参Action
xx.RemoveOnReleaseAddressableAsset<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//对象销毁
//(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default(TEventArg))
xx.OnDestroy<TEventArg>(action, args);
xx.OnDestroy(action, args); //指定参数类型的泛型可以不填，可以通过参数推断出
xx.OnDestroy(action); //无参Action
xx.RemoveOnDestroy<TEventArg>(action) //Remove时不传参，参数类型必须传，无法推断

//简单实例
void Start()
{
	cube.OnDestroy(Test4, 4);
}
private void Test4(GameObject arg1, int arg4)
{
	Debug.Log(arg4);
	cube.RemoveOnDestroy<int>(Test4);
}
```


* xx为绑定事件的对象组件，使用游戏对象的transform即可。
* TEventArg指定事件的参数类型，添加监听时可以不填，可以通过参数args推断出，移除监听时则必须显示指出。
* action是绑定的事件，根据事件类型（鼠标、碰撞、自定义事件），其方法的参数列表包含两部分，第一部分是事件本身的参数（PointerEventData、Collision），第二部分是参数列表TEventArg，可以通过值元组传入多个参数。

### 移除一类事件

移除鼠标/碰撞/触发/资源的一类所有事件。

```js
//API
//int customEventTypeInt, JKEventType eventType
RemoveAllListener(customEventTypeInt);
RemoveAllListener(eventType);
RemoveAllListener();

```


* customEventTypeInt/eventType为事件的类型，对应碰撞、鼠标事件对应的枚举类型或自定义事件的类型int值。
* 不填则移除所有事件。

## 使用值元组传递多个事件参数

通过ValueTuple封装一个简单的参数列表结构体。

```js
    void Start()
    {
        cube.OnClick(Test5, (arg1: 2, arg2: "test", arg3: true));
      	//等同于下一行代码，上一行更简便，参数类型可以自动推断出
        cube.OnClick(Test5, ValueTuple.Create<int,string,bool>(1,"test",true));
    }

    private void Test5(PointerEventData arg1, (int arg1, string arg2, bool arg3) args)
    {
        Debug.Log($"{args.arg1},{args.arg2},{args.arg3}");
        cube.RemoveOnClick<(int arg1, string arg2, bool arg3)>(Test5);
    }
```

## 自定义事件类型

以上鼠标、碰撞等事件的触发由事件工具结合特定的时机自动完成，如果希望自定义事件的触发逻辑，则需要添加新的事件类型,对应在适合的地方触发事件，此时事件工具的作用与事件系统类似，区别在于不需要为对象挂载脚本。

```js
//API
//(this Component com, int customEventTypeInt, Action<T, TEventArg> action, TEventArg args = default(TEventArg))
xx.AddEventListener<T, TEventArg>(customEventTypeInt, action, args);
xx.RemoveEventListener<T, TEventArg>(customEventTypeInt, action);
cube.TriggerCustomEvent<Transform>((int)myType.CustomType, transform);

//使用示例
    void Start()
    {
        cube.AddEventListener<Transform, int>((int)myType.CustomType, Test6, 1);
    }

    private void Test6(Transform arg1, int arg2)
    {
        Debug.Log(arg1.position = Vector3.zero);
        cube.RemoveEventListener<Transform, int>((int)myType.CustomType, Test6);
    }
    enum myType
    {
        CustomType = 0,
    }
```


* customEventTypeInt是自定义的事件类型，是一个int值，可以使用枚举对应事件的类型。
* T指明了自定义事件所使用的eventData，可以在触发的时候传入T以供使用，等同于Collision/Collider/PointerEventData。
* args是参数列表。

## 补充说明

* 事件工具针对的事件触发时都会提供eventData用于获取触发时的特定数据用于操作（有点类似于异步callback回调时传的那个参数），比如PointerEventData，因此他们与对象绑定，就算不传任何参数，触发时还是可以根据eventData去获取一些信息，比如碰撞发生的位置。
* 开发事件工具的目的在于快速为游戏对象添加一类事件的监听，而不需要为其手动挂载脚本（类似于button.OnClick.AddEventListener,但Unity只支持按钮的自动添加，而事件工具支持常见的所有事件类型），实际上会自动为其自动挂载JKEventListener脚本，其中有对应事件的监听方法以及内置碰撞/鼠标等事件的触发方法。
* 自定义事件类型是支持的，但此时事件的类型，触发需要自己实现。
* 事件系统作用在于解耦对象和事件触发的逻辑，让事件中心保存监听的方法，触发时不需要访问对象。而事件工具所负责的是一类与对象强关联的事件，用于解耦对象和事件监听添加的逻辑，不需要手动挂载脚本。二者联动的效果就是A使用事件工具直接为B添加事件监听C，C内部再通过事件系统包一层添加事件监听D，这样外界就可以通过直接访问事件中心触发D（可能的应用场景比如要给所有子弹添加碰撞分解效果，这样无论是事件监听添加还是事件的触发都可以在一个脚本中完成，不需要手动给所有子弹度挂载脚本，也不需要触发时访问所有子弹对象）。

# 状态机

游戏中的状态机（state machine）是一种在编程中常用的概念，它用于表示对象或系统的状态以及从一个状态到另一个状态的转换。在游戏中，状态机通常被用于表示游戏对象的状态，例如玩家角色的行动状态，或者敌人的攻击状态。每个状态都有一个特定的行为或属性，而状态之间的转换通常是由特定的事件触发的，例如按下某个按钮或达到某个条件。框架中提供了状态基类和转换功能逻辑。

## 状态机的初始化

使用状态机，本质就是持有状态机脚本的引用与其绑定，为此控制脚本（比如角色控制器PlayerController）需要持有状态机对象,其也是一类脚本资源，可以通过资源系统进行回收管理。

```js
//API
stateMachine.Init<T>(owner);
stateMachine.Init(owner);
//简单实例
public class PlayerController : MonoBehaviour, IStateMachineOwner
{
    StateMachine stateMachine;
    private void Start()
    {
        stateMachine = ResSystem.GetOrNew<StateMachine>();
      	//初始化时进入默认状态Idle
        stateMachine.Init<PlayerIdleState>(this);
  
      	//初始化时不进入默认状态
      	stateMachine.Init(this);
    }
}
```


* PlayerController需要继承IStateMachineOwner接口，目的是限制Init中填入的对象必须为接口子类;
* 宿主owner用于将PlayerController作为引用传递给stateMachine，因为stateMachine不继承MonoBehavior，想要获取PlayerController的引用相对麻烦，所以直接传递引用。
* T（PlayerIdleState）是初始的状态类，使用参数时会自动进入T状态，不填则状态机待机，等待进入新状态。

## 状态类

每一个状态实际都是一个状态类脚本，状态机通过切换调用其中的方法完成状态逻辑的切换，状态类脚本继承自状态基类StateBase，包含状态的初始化、进入、退出、Unity生命周期函数（虽然不继承MonoBehaviour，通过托管系统可以实现），初始化只执行一次，可以通过StateMachine调用state的Init方法传递过来的owner获取宿主的信息，比如在角色移动的相关状态中就可以获取Player的Transform组件。

```js
public class PlayerIdleState:StateBase
{
    Transform transform;
    public override void Init(IStateMachineOwner owner)
    {
        transform = ((PlayerController)owner).transform;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
```

## 状态的切换

在切换状态时，状态类会先被获取并存储起来供下次重复使用，当前状态的所有方法停止工作，切换到新状态的方法执行,在实际使用时，可以再封装一层ChangeState逻辑使用枚举与不同的状态类对应，简化代码。

```js
//API
//(bool reCurrstate = false)
stateMachine.ChangeState<T>(reCurrstate)
```


* reCurrstate指当前状态和要切换的状态相同时是否还要切换，默认为False相同状态不执行切换。

## 状态共享数据

为owner-stateMachine下的所有状态提供共享字典。

```js
public void ShareData()//状态类内
{
    //(string key, object data)
    RemoveShareData("test");
    ContainsShareData("test");
    AddShareData("test",1);
    int result = 0;
    TryGetShareData<int>("test", out result);
    UpdateShareData("test", 1);
    CleanShareData();
  
    stateMachine.CleanShareData();
  
}
```


* key,data是需要共享的字典数据，提供CRUD的API，共享数据可以在状态类内使用，也可以通过stateMachine调用API进行CRUD。

## 状态机状态清空与销毁

* 状态机停止工作Stop时会清空所保存的所有状态类放入对象池，状态机本身与宿主的引用仍旧保留，可供下次直接使用。
* 状态机销毁时Destroy除停止工作外会释放与宿主的引用关系，并将自己放回对象池回收利用。

```js
stateMachine.Stop();
stateMachine.Destroy();
```

# 场景系统

场景系统提供了正常加载场景和异步加载的若干API。

## 正常加载场景

```js
//API
//(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
SceneSystem.LoadScene(sceneName, LoadSceneMode mode = LoadSceneMode.Single);
//(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
SceneSystem.LoadScene(sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single);
//(string sceneName, LoadSceneParameters loadSceneParameters)
SceneSystem.LoadScene(sceneName, loadSceneParameters);
//(int sceneBuildIndex, LoadSceneParameters loadSceneParameters)
SceneSystem.LoadScene(sceneBuildIndex, loadSceneParameters);

//使用实例 加载SampleScene场景并Destroy当前场景
SceneSystem.LoadScene("SampleScene",LoadSceneMode.Single);
```


* sceneName对应BuildSetting中的场景名。
* sceneBuildIndex对应BuildSetting中的场景索引号。
* mode是场景的加载模式，默认为Single表示加载新场景会销毁当前场景，Additive则保留当前场景，将新场景加入到当前场景中。
* loadSceneParameters是场景加载的参数，除可指定加载模式外，还可指定优先级等，具体如图。

​![alt](https://uploadfiles.nowcoder.com/images/20230218/796382749_1676698492532/D2B5CA33BD970F64A6301FA75AE2EB22)​

## 异步加载场景

异步加载场景可在加载大规模场景时不阻塞主线程，而是通过协程或回调等方式在后台加载场景资源，并在加载完成后通知游戏主线程。

异步加载过程中主线程获取进度的方式有两种：

* 场景系统异步加载时会将加载进度传递到事件中心中，可以通过监听"LoadingSceneProgress"、"LoadSceneSucceed"事件获取加载进度。
* 异步加载提供了回调事件参数，可以通过传入回调函数获取加载进度。

```js
//API
//(string sceneName, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
SceneSystem.LoadSceneAsync(sceneName, callBack, mode);
//(int sceneBuildIndex, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
SceneSystem.LoadSceneAsync(sceneBuildIndex, callBack, mode);

//简单实例 异步加载场景SampleScene并实时输出加载进度，在加载完成时输出Success
//方式1 监听事件获取加载进度
SceneSystem.LoadSceneAsync("SampleScene");
//(float不写也行,"LoadingSceneProgress"、"LoadSceneSucceed"为固定名称)
EventSystem.AddEventListener<float>("LoadingSceneProgress", LoadProgress);
EventSystem.AddEventListener("LoadSceneSucceed");

//方式2 传入回调事件callBack
SceneSystem.LoadSceneAsync("SampleScene", LoadProgress);
EventSystem.AddEventListener("LoadSceneSucceed");

void LoadProgress(float progress)
{
  	Debug.Log(progress);
}
void LoadSceneSucceed()
{
  	Debug.Log("Success");
}
```


* sceneName对应BuildSetting中的场景名。
* sceneBuildIndex对应BuildSetting中的场景索引号。
* mode是场景的加载模式，默认为Single表示加载新场景会销毁当前场景，Additive则保留当前场景，将新场景加入到当前场景中。
* callBack是float参数无返回值回调事件，用于获取加载进度。

# Mono代理系统

Mono代理系统用于不继承MonoBehavior的脚本启用mono生命周期函数和协程，比如状态机里的状态类，场景系统异步加载时的协程，除代理系统外框架中的各系统都是静态工具类，需要使用Mono的相关方法则通过代理系统完成，因此也只有MonoSystem挂载在面板上，其内部实现是单例。

​![alt](https://uploadfiles.nowcoder.com/images/20230218/796382749_1676700223402/D2B5CA33BD970F64A6301FA75AE2EB22)​

## Mono生命周期函数

将需要在Update、LateUpdate。FixedUpdate实际执行的逻辑托管给MonoSystem。

```js
//(Action action)
MonoSystem.AddUpdateListener(action);
MonoSystem.RemoveUpdateListener(action);
MonoSystem.AddLateUpdateListener(action);
MonoSystem.RemoveLateUpdateListener(action);
MonoSystem.AddFixedUpdateListener(action);
MonoSystem.RemoveFixedUpdateListener(action);
```


* action是要在生命周期执行的无参无返回值方法。

## 协程

启动/停止协程。

```js
//API
//启动/停止一个协程
//(IEnumerator coroutine)
MonoSystem.Start_Coroutine(coroutine);
//(Coroutine routine)
MonoSystem.Stop_Coroutine(routine);

//启动/停止一个协程序并且绑定某个对象
//(object obj,IEnumerator coroutine)
MonoSystem.Start_Coroutine(obj, coroutine);
//(object obj,Coroutine routine)
MonoSystem.Stop_Coroutine(obj, routine);

//停止某个对象绑定的所有协程
MonoSystem.StopAllCoroutine(obj);

//停止所有协程
MonoSystem.StopAllCoroutine();
```


* coroutine是一个迭代器，定义了协程。
* routine是要停止的协程。
* obj是与协程绑定的对象，可以用于区分不同对象上的相同协程。

# 协程工具

提前new好协程所需要的WaitForEndOfFrame、WaitForFixedUpdate、YieldInstruction类的对象,避免GC。

```js
CoroutineTool.WaitForEndOfFrame();
CoroutineTool.WaitForFixedUpdate();
//(float time)
CoroutineTool.WaitForSeconds(time);
//(float time)不受TimeScale影响
CoroutineTool.WaitForSecondsRealtime(time);
//(int count=1)
CoroutineTool.WaitForFrames(count);
CoroutineTool.WaitForFrame();

//使用示例
private static IEnumerator DoLoadSceneAsync(...)
{
	yield return CoroutineTool.WaitForFrame();
}
```

# 扩展方法

框架提供了若干扩展方法用于快速调用与对象强关联的系统方法。

```js
//API
//比较两个对象数组,返回bool
//(this object[] objs, object[] other)
xx.ArraryEquals(other);

//调用MonoSystem添加/移除生命周期函数
//(this object obj, Action action)
xx.AddUpdate(action);
xx.removeUpdate(action);
xx.AddLateUpdate(action);
xx.RemoveLateUpdate(action);
xx.AddFixedUpdate(action);
xx.RemoveLateUpdate(action);

//调用MonoSystem启动/停止协程（绑定此对象）
//(this object obj, IEnumerator routine)
xx.StartCoroutine(routine);
//(this object obj, Coroutine routine)
xx.StopCoroutine(routine);
xx.StopAllCoroutine();

//判断GameObject是否为空，返回bool
xx.IsNull();

//当前对象放入对象池
//(this GameObject go)
xx.GameObjectPushPool();
//(this Component com)
xx.GameObjectPushPool();
//(this object obj)
xx.ObjectPushPool();
```

# 本地化系统

用于切换不同语言对应的文字素材和图片素材，主要用于UI。

## 本地化配置文件的创建

project面板右键创建Localzation Config，通过SO的方式记录语言配置，红色框为一类素材的key，对应下属若干不同语言的素材，支持文字string或者图片Sprite内容，使用时通过调用这里的资源进行切换，可以作为全局或者专属于某一UI对象的本地化配置文件（即持有此config的SO并通过key和languagetype获取本地化内容）。

​![alt](https://uploadfiles.nowcoder.com/images/20230930/796382749_1696065707645/D2B5CA33BD970F64A6301FA75AE2EB22)​

## 全局配置

​![alt](https://uploadfiles.nowcoder.com/images/20230930/796382749_1696067447989/D2B5CA33BD970F64A6301FA75AE2EB22)​

将创建的Config拖拽给JKFrame下的LocalizationSystem组件，全局的本地化配置绑定完成，通过修改LocalizationSystem的LanguageType修改语言。（可以在运行时下修改全局配置）

## API

```js
//切换全局配置的当前语言类型（面板上显示的那个）
LocalizationSystem.LanguageType = LanguageType.SimplifiedChinese;
//注册/注销语言更新时触发的事件（含有LanguageType参数的无返回值方法）
LocalizationSystem.RegisterLanguageEvent(Action<LanguageType> action)
LocalizationSystem.UnregisterLanguageEvent(Action<LanguageType> action)
//获取全局配置文件的某一语言下的数据（文本/图片）
LocalizationSystem.GetContent<T>(string key, LanguageType languageType)
//继承UIWindowBase的窗口脚本可以重写语言更新事件
override void OnUpdateLanguage(LanguageType languageType)

//使用案例
//1.通过脚本直接获得全局本地化配置的数据内容
//文本string
string info = LocalizationSystem.GetContent<LocalizationStringData>("标题", LanguageType.SimplifiedChinese).content;//指定中文
string info = LocalizationSystem.GetContent<LocalizationStringData>("标题", LocalizationSystem.LanguageType).content;//当前全局本地化配置的语言
//Sprite图片
Sprite image = LocalizationSystem.GetContent<LocalizationImageData>("标题图片", LanguageType.SimplifiedChinese).content;

//2.通过Collecter由拖拽的方式绑定UI组件和语言配置（见下一小节）
//3.通过重写OnUpdateLanguage定制语言更新时的事件触发（见下一小节）
   
```


* action是一个含有languageType的单参数无返回值方法，用于结合传入的语言类型定制触发事件逻辑。
* 泛型T（LocalizationStringData/LocalizationImageData）用于限定GetContent返回的数据类型，目前支持string和Sprite，可以进行拓展。
* key是本地化配置文件SO中的数据key。
* LocalizationSystem.LanguageType是当前游戏的语言类型，修改会触发索引中的语言更新方法，进而触发所有窗口的语言更新事件修改语言类型。

## UI特化工具及局部配置

​![alt](https://uploadfiles.nowcoder.com/images/20240321/796382749_1710991072073/D2B5CA33BD970F64A6301FA75AE2EB22)​

* 在UI框架中继承UIWindowBase的窗口类会自动持有一个本地化配置A用于窗口的局部配置（可用可不用，只是提供了一个数据传入的接口）。
* 方便起见，，直接通过面板拖拽的方式转递对象和其对应的配置数据key（任一语言即可），即完成了本地化配置，无需通过脚本访问（比如Title文本组件对应配置中的标题key）。注意，此时的Localization Config是一个专属于此UI的局部配置文件（且与持有的本地化配置A可以不同）。

PS:当局部配置找不到对应的key时，底层规定会去全局本地化配置表里寻找。

​![alt](https://uploadfiles.nowcoder.com/images/20230930/796382749_1696067638536/D2B5CA33BD970F64A6301FA75AE2EB22)​

集成了UI_WindowBase的UI窗口类可以通过重写OnUpdateLanguage方法定制语言更新时的事件触发，比如文字拼接部分更新。

```js
    public Text test;
    protected override void OnUpdateLanguage(LanguageType languageType) {
        string info = LocalizationSystem.GetContent<LocalizationStringData>("标题", languageType).content;
        info += "test";
        test.text = info;
    }
```

## 拓展

尽管本地化系统目前仅支持文字和Sprite的切换，但是对于音效，配音等资源的切换也可以很方便拓展，这部分功能就不做预制了，由开发者自行拓展，以下是拓展思路。

```js
//拓展API
//获取全局配置文件的某一语言下的数据（文本/图片）
LocalizationSystem.GetContent<T>(string key, LanguageType languageType)

//拓展位置  LocalizationData.cs
public abstract class LocalizationDataBase
{
}
public class LocalizationStringData : LocalizationDataBase
{
    public string content;
}
public class LocalizationImageData : LocalizationDataBase
{
    public Sprite content;
}
```

* 在本地化系统的内部实现中，GetContent的泛型参数T指定了本地化保存的数据类型，内置了LocalizationStringData和LocalIzationImageData两种数据类型，分别持有string成员和Sprite成员，对应文本和图片数据。
* 有两种修改思路，一种是在已有的两个个数据类型添加额外的数据成员,比如一个武器在UI上的显示除了有描述内容还有武器的类型string。

```js
public class LocalizationStringData : LocalizationDataBase
{
    public string content;
  	public string type;
}
```

* 另外一种是直接继承抽象类LocalizationDataBase写一个新类，比如还是武器类型和武器描述的UI本地化数据,其实两种方式本质也没啥区别，只是说明数据类数量和类内的数据成员都可以扩展满足开发者想要的需求。

```js
public class LocalizationWeaponData : LocalizationDataBase
{
    public string content;
  	public string type;
}
```
