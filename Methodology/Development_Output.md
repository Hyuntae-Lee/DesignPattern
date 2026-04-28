기준 : IEC 62304 + ISO 13485 + ISO 14971

### 전체 구조 ###
1. 핵심 흐름
   <pre>
   URS
    ↓
   SRS
    ↓
   Software Architecture
    ↓
   Detailed Design
    ↓
   Implementation
    ↓
   Verification (Test)
    ↓
   Validation
   </pre>
2. 모든 핵심 흐름 학목들의 옆에서 붙는 것
   - Risk Management (전 구간 영향)
   - Traceability Matrix (전 연결 관리)

### 요구사항 계층 ###
1. URS (User Requirements Specification)
   - 사용자 관점 요구사항
   - Intended Use 포함
   - 연결
     - SRS
     - Validation
2. SRS (Software Requirements Specification)
   - 소프트웨어 기능 요구사항
   - 성능, 인터페이스, 안전 요구 포함
   - 입력
     - URS
     - Risk Control (위험통제 요구)
   - 출력 연결
     - Software Architecture
     - Test Case (Verification)
3. System / Derived Requirements (중요: 실무에서 빠지기 쉬움)
   - Risk에서 내려온 요구사항
   - 내부 기능 (로그, 디버그 등)
   - 연결
     - Risk Management File
     - SRS에 포함 or 별도 관리

### 위험관리 트랙 (병렬 구조) ###
1. Risk Management Plan
2. Hazard Analysis / FMEA
3. Risk Control Measures
   - 핵심 연결
     - Risk Control → SRS requirement로 내려감
4. Risk Management Report
   - 최종적으로 모든 Risk가 테스트로 검증되었는지 확인

### 설계 계층 ###
1. Software Architecture Document
   - 시스템 구조
   - 모듈 분리
   - 데이터 흐름
   - 연결
     - SRS → Architecture
2. Detailed Design Specification
   - 클래스/함수 수준 설계
   - 상태 다이어그램 등
   - 연결
     - Architecture → Detailed Design
3. Interface Specification
   - 외부 장치 / MCU / API 인터페이스
   - 연결
     - SRS 인터페이스 요구

### 구현 계층 ###
1. Source Code
2. Code Review Record
3. Static Analysis Report
   - 연결:
     - Detailed Design → Code
4. Configuration Management
   - 형상관리 계획
   - 버전 관리 기록
   - 기준
     - ISO 13485

### 검증 (Verification) ###
1. Test Plan
2. Test Case Specification
3. Test Report
   - 연결
     - SRS → Test Case (핵심)
     - Risk Control → Test Case (필수)
4. Unit Test / Integration Test / System Test
   - 기준:
     - IEC 62304

### 검증 (Validation) ###
1. Validation Plan
2. Validation Report
   - 연결
     - URS → Validation
3. Usability Engineering File
   - 사용성 평가
   - 기준
     - IEC 62366

### 추적성 (핵심 축) ###
1. Traceability Matrix
   - 최소 포함 관계:
     <pre>
     From	|   To
     ---------------------
     URS	|   SRS
     SRS	|   Design
     SRS	|   Test
     Risk	|   SRS
     Risk	|   Test
     URS	|   Validation
     </pre>
   - 이 문서 하나로 “심사 대응 50% 해결”

### 변경관리 / 품질 ###
1. Change Request / Change Log
2. CAPA
3. Issue Tracking
   - 연결
       - 변경 → 영향받는 SRS / Test / Risk 모두 업데이트

### 릴리즈 / 유지보수 ###
1. Release Note
2. Software Version Description
3. Anomaly List

### 실무용 구조
<pre>
01_Requirements
  - URS
  - SRS

02_Risk_Management
  - Risk Plan
  - Hazard Analysis
  - Risk Report

03_Design
  - Architecture
  - Detailed Design
  - Interface Spec

04_Implementation
  - Source Code
  - Code Review
  - Static Analysis

05_Verification
  - Test Plan
  - Test Cases
  - Test Report

06_Validation
  - Validation Plan
  - Validation Report
  - Usability

07_Traceability
  - Traceability Matrix

08_Configuration
  - CM Plan
  - Version History

09_Change_Control
  - Change Requests
  - CAPA
</pre>

### 핵심 포인트 (중요) ###
1. SRS는 중심 허브
   - 거의 모든 문서가 SRS와 연결됨
2. Risk는 별도지만 SRS로 흘러들어감
3. Test는 SRS + Risk를 모두 커버해야 함
4. URS는 Validation으로 닫힘

### 한 줄 정리 ###
👉 “의료기기 소프트웨어 산출물 체계는
URS → SRS → Design → Code → Test → Validation
- Risk + Traceability로 완성된다.”