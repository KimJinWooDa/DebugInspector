# DebugInspector

# Overview
DebugInspector는 스크립트의 Debug 관리를 편리하게 수정하기 위해 고안된 Util Tool입니다. 

DebugInspector를 사용하면 모든 Debug 문을 검사, 편집, 관리할 수 있으므로 개별 스크립트를 탐색할 필요가 없습니다.

# Why DebugInspector?
- 중앙 집중식 관리: 디버그 문을 보거나 수정하기 위해 여러 스크립트를 열 필요가 없습니다. 한 곳에서 모든 디버그를 한눈에 파악할 수 있습니다.
- 효율적인 디버그 검색: 내장된 검색 기능으로 디버그 문을 빠르게 찾을 수 있습니다.
- 즉석 편집: 인스펙터에서 바로 디버그 문을 수정하고 변경 사항이 스크립트에 반영되는 것을 확인할 수 있습니다.
- 폴더 기반 검사: 폴더를 기준으로 스크립트를 필터링하여 프로젝트의 특정 부분에 집중할 수 있습니다.

# How to Use
## Tool 열기
- Unity 상단 표시줄에서 창으로 이동하여 디버그 인스펙터를 선택합니다.
## 폴더 선택
- 폴더 선택 드롭다운을 사용하여 프로젝트에서 특정 폴더를 선택합니다.
- 선택한 폴더에 있는 Debug가 포함된 스크립트를 자동으로 나열합니다.
## 스크립트 검색
- 검색 창을 사용하여 이름별로 스크립트를 필터링할 수 있습니다.
## Debug 편집
- 다음 및 이전 버튼을 사용하여 스크립트의 Debug사이를 탐색할 수 있습니다.
- 제공된 Text Field에서 디버그 문을 직접 편집합니다. 여기서 변경한 내용은 **스크립트에 반영**됩니다.
- 편집 저장을 클릭하여 변경 사항을 확인합니다.
- Remove 버튼을 클릭하여 스크립트에서 Debug 문을 삭제합니다.
## Custom Debug Management
- 유니티에서 제공하는 Debug를 제외한 유저가 커스터마이징한 Debug또한 관리 할 수 있습니다.

# Other Features
- 스크립트 열기 버튼을 사용하여 해당 스크립트를 열 수 있습니다.
- 모든 디버그 보기 옵션을 사용하면 선택한 스크립트의 모든 디버그 문을 자세히 살펴볼 수 있습니다.

# Precautions
- 백업: 항상 스크립트의 백업을 유지하거나 버전 관리 시스템을 사용하세요. DebugInspector는 안전하도록 설계되었지만 원본 스크립트의 사본을 보관하는 것이 좋습니다.
- 성능: 스크립트가 많은 대규모 프로젝트에서는 디버그 문을 검색할 때 약간의 지연이 발생할 수 있습니다. 더 빠른 결과를 얻으려면 폴더별로 필터링하는 것을 고려하세요.
- 호환성: 이 툴은 특정 Unity 버전에서 테스트되었습니다. 호환되는 버전을 사용하고 있는지 확인하거나 별도의 프로젝트에서 먼저 테스트하세요.

# 간단한 윈도우 창
![image](https://github.com/KimJinWooDa/DebugInspector/assets/76438011/b9ec4457-8fee-4b59-9160-5a8e65b4147f)


- 매우 직관적으로 제작되어 쉽게 사용이 가능합니다.

2023.09.26 업데이트
