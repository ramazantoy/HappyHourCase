# HappyHourCase

HappyHourCase projesi, **SOLID prensiplerine** uygun, esnek ve modüler bir mimari ile geliştirilmiştir. Projede durum yönetimi için **StateMachine** yapısı, bağımlılık yönetimi ve performans optimizasyonu için ise **Zenject** kullanılarak interface binding ve ObjectPooling işlemleri gerçekleştirilmiştir. Ayrıca, proje kapsamında oklar, düşmanlar ve oyuncu kontrolü için kalıtım yoluyla genişletilebilir yapı oluşturulmuştur.

## Proje Özellikleri

- **SOLID Prensipleri:** Kodun sürdürülebilirliğini sağlamak amacıyla SOLID prensiplerine uygun tasarım uygulanmıştır.
- **StateMachine:** Oyun içindeki durum yönetimi, esnek bir yapı ile kontrol edilmekte; farklı durumlara (örneğin, atış, hareket, hasar alma) uygun davranışlar kolaylıkla tanımlanabilmektedir.
- **Zenject ile Dependency Injection & ObjectPooling:**
  - **Interface Binding:** Zenject kullanılarak, uygulama genelinde kullanılacak interface’ler ile somut sınıflar arasında sağlam bir bağ kurulmuştur.
  - **ObjectPooling:** Performans optimizasyonu için, nesnelerin yeniden kullanılmasını sağlayan ObjectPooling mekanizması entegre edilmiştir.
- **Modüler Ok Yapısı:**
  - **BaseArrow:** Okların temel ayarları (base hasar, hareket hızı, yer çekimi, hava sürtünmesi, rotasyon hızı) burada tanımlanır.
  - **Türetilmiş Arrow Sınıfları:** Her ok tipi için özel davranış ve özellikler (örneğin, BounceCount, BurnDamage) türetilmiş sınıflarda uygulanarak esnek bir yapı sunulmuştur.
- **Modüler Düşman Yapısı:**
  - **EnemyBase:** Tüm düşmanlar için ortak özellikler ve davranışlar bu sınıfta tanımlanır.
  - **EnemyYBot:** Şu anda EnemyBase ile benzer yapıdadır; ancak gelecekte farklılaşacak özelliklerin eklenebilmesi için ayrı bir sınıf olarak tasarlanmıştır.
- **PlayerController & State Yönetimi:**
  - **PlayerController:** Oyuncunun hareket etmesi, state değiştirmesi ve ateş etmesi gibi temel işlemleri gerçekleştiren ana kontrol sınıfıdır.
  - **State Yapısı:** Oyuncu davranışları, `IdleState`, `MoveState` ve `ShootState` gibi durum sınıfları ile yönetilmektedir. Böylece oyuncunun animasyonları, hareketi ve saldırı eylemleri ayrı modüller halinde ele alınarak esnek bir yapı sağlanmıştır.
  - **Input & Animasyon Yönetimi:** `VariableJoystick` üzerinden alınan girdilerle oyuncu hareket ettirilirken, Animator aracılığıyla animasyonlar kontrol edilmektedir.
  - **Asenkron Saldırı Mekaniği:** `ShootState` içinde asenkron saldırı rutinleri, cancellation token kullanılarak yönetilir. Bu yapı, oyuncu farklı bir state’e geçtiğinde saldırı işleminin kesilmesini sağlar.
  - **Balistik Ok Fırlatma:** Okların hedefe doğru balistik (parabolik) hız vektörü hesaplamaları ile fırlatılması sağlanır; böylece gerçekçi bir atış simülasyonu elde edilir.

## Projede Kullanılan Assetler

- **Archer Girl:** [3D Model](https://sketchfab.com/3d-models/archer-girl-373ea6d232e741b781658915b66ecaea)
- **Player:** Mixamo
- **Enemy:** Mixamo
- **Animasyonlar:** Mixamo
- **Joystick:** [Unity Asset Store](https://assetstore.unity.com/packages/tools/input-management/joystick-pack-107631#content)
- **Trails:** [Unity Asset Store](https://assetstore.unity.com/packages/vfx/trails-vfx-242572)
- **Environment:** [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/lowpoly-environment-extreme-pack-238098)
- **Ok Sesi:** [OpenGameArt](https://opengameart.org/content/bow-arrow-shot)
- **Alev Particle:** [Unity Asset Store](https://assetstore.unity.com/packages/vfx/particles/free-asset-vfx-particles-flame-pack-263899)
- **SkyBox:** [Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/sky/customizable-skybox-174576)
- **UniTask:** [GitHub](https://github.com/Cysharp/UniTask)
- **Zenject:** [GitHub](https://github.com/modesttree/Zenject)

## Bölüm Açıklamaları

### PlayerController

PlayerController, oyuncunun temel hareket, durum (state) yönetimi ve saldırı işlemlerini yöneten ana sınıftır.  
Öne çıkan özellikleri:

- **State Yönetimi:**  
  - Oyuncu davranışları `IdleState`, `MoveState` ve `ShootState` sınıfları ile yönetilir.
  - Her durum, oyuncunun animasyonlarını, hareketini ve saldırı eylemlerini kontrol eder.
  - Durum geçişleri, mevcut state'in `Exit()` metodunun çağrılması ve yeni state'in `Enter()` metoduyla başlatılması şeklinde gerçekleştirilir.
  
- **Girdi ve Animasyon:**  
  - `VariableJoystick` üzerinden alınan input ile oyuncunun hareketi sağlanır.
  - Animator aracılığıyla hareket hızı ve saldırı animasyonları kontrol edilir.
  
- **Dependency Injection & Object Pooling:**  
  - Zenject kullanılarak, `IEnemyManager` gibi bağımlılıklar ve farklı arrow türleri için (BasicArrow, BounceArrow, BurnArrow) object pooling yapısı enjekte edilir.
  
- **Asenkron Saldırı Mekaniği:**  
  - `ShootState` içinde asenkron saldırı rutinleri, UniTask ve cancellation token kullanılarak yönetilir.
  - Atış hızı, oyuncunun sahip olduğu yeteneklere göre dinamik olarak hesaplanır.
  
- **Balistik Hesaplama:**  
  - Okların hedefe doğru fırlatılması için balistik (parabolik) bir hız vektörü hesaplanır. Böylece, gerçekçi bir ok atış simülasyonu elde edilir.


### ArrowBase & Türetilmiş Arrow Sınıfları

![image](https://github.com/user-attachments/assets/cd58e22b-244b-49ac-8d97-cfdddec84051)

Okların temel ayarları `ArrowBaseDataContainer` üzerinden yapılmaktadır. **BaseArrow** sınıfı, temel ok özelliklerini barındırırken; her ok tipi için türetilmiş sınıflarda (örneğin, farklı BounceCount veya BurnDamage değerleri) kendine özgü ek özellikler tanımlanmıştır. Bu yapı, farklı ok davranışlarını kolayca genişletme imkanı sağlar.

### Enemy Yapısı

![image](https://github.com/user-attachments/assets/b1383bd9-4cb3-4263-a7cc-704d00cd6249)

Düşmanlara ait ayarlar, `EnemyDataContainer` üzerinden yapılmaktadır. Şu an için sadece can geçişlerinde alınacak renk ayarları bulunmaktadır. Ek olarak:
- **EnemyBase:** Tüm düşmanlar için ortak özellikleri ve davranışları içerir.
- **EnemyYBot:** Şu anda EnemyBase ile aynı yapıya sahip olup, ileride farklılaşacak özelliklerin eklenmesine olanak tanıyacak şekilde tasarlanmıştır.

### GameManager

![image](https://github.com/user-attachments/assets/9faec6a1-ca43-40ef-bed2-3a169c3d7121)

Oyun başlatıldığında, rastgele bir harita oluşturulmaktadır. Bu sistem, istenmeyen haritaların kolayca kaldırılmasını veya değiştirilmesini mümkün kılmaktadır.

## Ek Notlar

- **16:9 Potrait ** Farklı cihazlarda farklı görünüm ile karşılaşılırsa Cihazın kamerasının çözünürlüğüne göre referans çözünürlükle karşılaştırılıp objelerin scale'ini büyüten bir script projeye eklenebilir.

## Game Play Video

 
 https://github.com/user-attachments/assets/666f0875-a91f-46e7-905a-4a3d3b2da4cb 



## Arrow Project Tile (Düşük hız)


https://github.com/user-attachments/assets/9b3d4eb3-4883-40d9-93f2-951a735fa32f






