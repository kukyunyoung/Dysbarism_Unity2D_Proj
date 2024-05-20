유니티 2D 프로젝트 Dysbarism
=============

평소에 즐겨하고 좋아하는 게임장르는 Dungreed, Skul : the hero slayer, 던전앤파이터, 록맨 등 2D 게임에 흥미가 많은 편입니다.

직업학원에 다니게 되면서 프로젝트 제작을 할 기회가 생겼고, 해봤던 게임들에서 재미있었다 라고 생각하는 요소들을 뽑아 게임을 만들었습니다.

기간 : 2024.01.11~2024.03.17

영상 https://youtu.be/6Gf9S-YIjh8

## 시스템

플레이어는 게임이 시작되면 축복을 한가지 선택할 수 있습니다.

축복과 아이템을 통해 바닷속을 탐험하는 컨셉으로 제작 하였습니다.

아이템을 통해 플레이어 스탯을 강화하고 몇가지의 무기를 구현해 스탯에 맞게 적절한 플레이가 가능하도록 하였습니다.

상점NPC, 무기강화NPC를 만들어서 아이템을 수집해 공략할 수 있게 하였습니다.

플레이어가 몬스터에게 공격받을때 상태이상을 유발시켜 공략이 쉽지 않도록 레벨디자인을 하였습니다.

매 게임마다 맵을 랜덤으로 배치시켜 여러차례 플레이해도 새로운 플레이경험이 될 수 있도록 하였습니다.


## 조작법

이동 : WASD  
공격 : 마우스 좌클릭  
축복 : 마우스 우클릭  
상호작용 : E  
스탯창 : X  
인벤토리 : V  

## 개발요소

- 플레이어 축복

### 대쉬

유저의 마우스 방향쪽으로 짧은 대쉬를 사용함.

사용 횟수는 스택구조로 관리되며 최대3회 사용가능하고 시간이 지나면 사용횟수가 회복됨.

![dash](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/8f014b40-e24c-4c59-a78d-3c24269bdb19)

### 침착

플레이어를 포함한 모든 게임요소들을 느리게 동작하게 함.

게이지 구조로 사용가능 여부가 판단되며, 게이지가 40% 이하일때는 사용이 불가하도록 함.

![slow](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/4b46ec70-f07e-41b1-88ec-d420558be586)

### 분노

플레이어의 공격력을 50% 강화하고 방어력을 0으로 이동속도를 2배 빠르게 함.

20초의 쿨타임 12초의 지속시간을 가짐.
  
![rage](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/0c96ef5e-636f-480b-adcb-4ed63dc332bf)  


- NPC

### 상점 NPC

매 게임마다 약 10개의 아이템 데이터를 불러와 상점에 2~4개 랜덤하게 배치.  
아이템 데이터는 Scriptable Object로 관리됨.  
구매시에 상점에는 품절 표시와 함께 더 구매할 수 없게 되고, 구매한 아이템은 인벤토리의 빈공간 제일 앞에 저장됨.

### 축복 NPC

게임 시작했을때와 같은 동작으로 플레이어 축복을 변경할 수 있게 해줌.

### 아이템 강화 NPC

아이템 데이터에는 다음등급의 아이템 정보도 갖고있음.  
이를 활용해서 일정량 재화를 지불하면 다음 무기로 업그레이드 할 수 있는 NPC 만듬.

  
- 몬스터 AI

몬스터의 종류는 일반몬스터 5개, 보스몬스터 2개 총 7개의 몬스터를 작업 했습니다.

상속구조를 최대한 활용해 움직임, 사망 같은 처리는 공통적으로 사용할 수 있게 하였고 플레이어 추격, 공격 등은 각각 메소드를 작성해 활용하였습니다.

### 일반 몬스터

1. Piranha

![piranha](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/bcaeaf69-a51d-4c36-a262-382c1caaf07d)

가장 일반적인 몬스터 격이며 공통된 움직임을 설명할 수 있는 몬스터.

Idle 상태일때는 좌우를 배회하며 플레이어를 찾아다니는 행동을 함.

탐색범위 내에 플레이어가 들어오거나 공격을 받았을경우 Trace 상태가 되어 플레이어를 추격함.

공격시에 플레이어에게 일정확률로 출혈 상태이상을 부여함 (화면이 빨갛게 점등)

2. Shark

![shark](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/5490db2c-00b0-47cc-adb1-e41060cb3fd0)

![sharkBlood](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/d272fd99-a7a6-4549-8591-70e321805ca9)

플레이어가 출혈상태가 되면 맵에 존재하는 모든 상어 몬스터들이 Trace 상태가 되어 플레이어에게 돌진함.

3. Octopus

![octopus](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/1cad24c7-7a3b-4c80-941f-71c7b1357da0)

탐색범위가 다른 몬스터들보다 넓음. 

Trace 상태가 되면 제자리에서 전조 애니메이션이 나오며 그 후 플레이어 위치로 돌진공격 함.

4. JellyFish

![jellyFish](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/e967a763-6131-4770-9203-63d22e957e2e)

Idle 상태에서는 위아래로 배회함.

Trace 상태에서도 느린 이동속도.

플레이어를 공격하면 감전 상태이상을 부여함 (화면이 노랗게 점등)

5. BlowFish

![blowfishgif](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/eea6e5ca-4a07-4871-96f8-158966108dda)

Trace 상태가 되면 몸을 부풀리며 플레이어에게 돌진함.

죽거나 플레이어와 닿게 되면 자폭공격을 시도함.

자폭공격을 하면 상하좌우 방향으로 가시가 발사되며 가시에 맞게되면 중독 상태이상을 부여함 (화면이 보라색으로 점등)

### 보스몬스터

1. GiantCrab

![bubble,niddle](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/733e0870-8672-4184-ba8a-329fab36d5d7)

![ground,hand](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/cacdb1de-bca4-41d9-9de5-3c9fbd390a46)

코루틴과 상태 패턴을 활용하여 행동패턴을 만듬.  
플레이어와 만나서 공격상태 -> n초 후 패턴 랜덤실행 -> 대기상태 -> n초 후 패턴 랜덤실행 -> ...

공격마다 콜라이더를 생성해 공격범위를 설정함.

2. EvilEye

두가지 기믹을 만들어 보스몬스터 라는 느낌을 주고자 함

멘탈 시스템 : 플레이어가 이블아이 쪽으로 마우스를 향하고 있으면(바라보고 있으면) 멘탈게이지가 차오르며 100이되면 즉사급 데미지를 받음

![hivementalgif](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/87c2c6d5-4f9a-4dfc-b17d-412bf862fdd8)


공격 패턴중 멘탈 게이지를 30% 채우는 기믹.

몬스터를 등짐으로써 회피할 수 있음.

![hivementaldie](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/2e28f703-8287-49dc-8068-58eddf02809c)

게이지가 90%가 넘어갈 때 부터 Post-Process 기법으로 카메라 효과를 줘서 극적인 연출을 함.
  
페이즈 시스템 : HP가 50%가 되면 무적상태가 되며 부하를 소환해 싸우게 하고 피를 회복함.  
부하를 전부 죽이면 회복 중단하고 다시 공격페이즈

그 외 가시발사, 폭발 공격이 있음.

> 해당 작업을 통해 상속구조가 어떻게 작동하는지 명확하게 이해가 가능했고
> 간단하지만 패턴을 만들어봄에 있어서 전투 시스템을 설계할때 어떻게 설계 해야할지 감을 익힘.

- 아이템, 아이템 데이터

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/b9d238d9-079e-49cb-a169-b7f7be628db8)

아이템과 아이템 데이터는 전략 패턴을 사용하여 구현함.

> 해당 작업을 통해 구현 부분에서는 복잡함을 느꼈지만 추후에 아이템 추가가 필요한 상황에서는 매우 간단하게 추가가 가능하도록 구현가능함을 알게됨.

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/60bc2b20-8807-4043-a05c-22561ea27cee)

무기아이템 3개, 장신구아이템 3개, 소모품아이템 5개를 만듬.

무기아이템은 플레이에 직접적으로 영향을 줌 (총을 쏜다던가, 칼을 휘두른다거나)

장신구 아이템은 스탯을 올려줌.

소모품 아이템도 스탯을 올려줌. 장신구아이템과 다른점은 1회성으로 오르기때문에 상승폭을 크게 잡았고 인벤토리 슬롯 한개에 여러 아이템을 담을 수 있다는 점.
  
- 인벤토리

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/c281c288-24ab-4a50-b68d-63921c9c6068)

### 인벤토리 슬롯은 사용자와 클릭, 드래그로 상호작용 할 수 있음.

인벤토리 슬롯의 아이템을 우클릭 할 경우에  
그 슬롯의 아이템이 장착아이템인지 소모품아이템인지 판별해  
장착아이템이면 장비아이템 장신구아이템 인지 판별후 장착  
소모품아이템이면 바로 사용되어 스탯이 오르거나 HP가 회복되는등의 작용.

아이템정렬 버튼은 Trim과 Sort가 있음

### Trim  
아이템 타입을 고려하지 않고 빈 슬롯을 찾아 맨 앞자리부터 채우는 방식

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/8b3706b4-d70f-4198-aeda-6f81c7928df2)

### Sort  
아이템 타입을 고려해 소모품-장비-장신구 순서로 정렬함

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/abca4dcb-66db-4003-b610-5673019c632e)

### 마우스 오버 시 필터효과  
슬롯이 파랗게 하이라이트 되고 아이템 툴팁이 생김

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/07a9194e-04fd-4323-a742-86097c35e738)

### 아이템 드래그 앤 드롭으로 버리기

![image](https://github.com/kukyunyoung/Dysbarism_Unity2D_Proj/assets/71830573/805a9b20-04b3-47aa-849d-e7bef7c7f427)


> 인벤토리와 유저의 상호작용은 InventoryManager.cs  
> 인벤토리의 직접적인 데이터 관리는 Inventory.cs
> 해당 작업을 통해 유지보수 할 때 역할이 구분되어 어느 기능이 어디있는지 확인하기 편했음.

- 데이터 저장

### Json포맷  
인벤토리 데이터  
플레이어 데이터(HP, HPMax, 스탯)  
소지 재화  
플레이 타임  

데이터는 각 맵의 출구를 드나들때, 사망했을때, 클리어 했을때 저장되게 함.

> 간소하게나마 저장기능을 구현해 봤는데 서버가 있다면 Json파일을 업로드하고 다시 받아오는 식으로 활용할 수 있을것으로 기대함.

## 마치며

깃허브 용량 문제로 업로드를 못하고 있다가 작업이 끝나고 나서야 업로드를 하였는데 두서없이 정리된 느낌이 있어서 아쉬움이 있습니다.  
2개월동안 열심히 공부하고 검색하며 만든 게임이 어느정도 형태를 갖췄다고 생각하니 성취감을 느낄 수 있는 프로젝트였다고 생각합니다.  
아직 초보단계라고 생각해 앞으로 또 새로운 프로젝트를 경험하며 실력을 쌓을 수 있도록 노력하겠습니다.  
