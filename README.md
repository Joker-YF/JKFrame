# README

JKFrame2.0设想的是为独立游戏服务的，小团队甚至只有一个人的情况下使用的！

JKFrame2.0比较像是个工具箱，除了UI窗口外的大多情况下，你并不需要继承什么类或接口，而提供的功能也不是非用不可~所以比较接近工具箱

很多功能更像是插件的感觉~如：

	SaveSystem.SaveObject(object)保存某个存档数据
	
	AudioSystem.PlayBGAudio(audioClip)播放某个背景音乐

**主要功能系统：**

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

**其他功能：**

1. 状态机：脚本逻辑状态机
2. 事件工具：给物体绑定 碰撞、触发、点击、拖拽、自定义等事件
3. 协程工具：协程避免GC
4. Excel和ScriptableObject互转功能：支持Unity预制体、Sprit等资源配置，支持GUID模式（就是不依赖路径，可以更改预制体路径），支持SubAsset(如就是切割图片后的精灵)

**内置：**

1. 依赖Odin插件，方便做一些序列化和调试
2. JKLog，日志

**团队**

Joker

Parks

**社群**

QQ群：654336425

作者：739554159（Joker）

使用手册：http://www.yfjoker.com/JKFrame/

**上线案例**

曹军来袭：https://store.steampowered.com/app/2452940/
