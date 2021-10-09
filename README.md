# Bitmap-Editor

개발 툴: Visual Studio 2017

개발 언어: C# winform

목적: ucGUI의 listbox의 아이콘 비트맵용 배열을 자동생성하기 위한 프로그램

사용 방법:
1. Control 그룹의 Row,Column을 편집하여 bitmap 크기를 결정
2. Bit Per Color로 사용할 색의 수를 결정 (BitPerColor=n일 때 색의 수=2^n)
3. 색 리스트의 항목을 더블클릭하여 색을 결정
4. 비트맵 편집
  - Brush/Fill 토글버튼을 누르고 비트맵 칸을 클릭하여 원하는 그림을 그림
  - Brush=선택한 칸만 색칠, Fill=선택한 칸과 연결되면서 색이 같은 모든 영역을 색칠
  - 색 리스트의 항목을 선택하여 Brush/Fill할 색을 결정
5. Make버튼을 눌러 565RGB로 변환한 배열값을 생성


# UI

![BitmapEditor](https://user-images.githubusercontent.com/28644565/136661941-fc9bc15a-a7a1-4f7d-a158-64f624ca4458.PNG)

![BitmapEditor2](https://user-images.githubusercontent.com/28644565/136661943-3bcb1afe-f0ef-4560-b3fd-60e7e174390d.PNG)
