# ISO 19157 표준의 RuleID/측정ID 부여 방식 규정 검토 결과

## 1. 검토 개요

ISO 19157 시리즈 표준에서 RuleID나 측정ID(Measurement ID) 부여 방식에 대한 명시적 규정이나 규칙이 있는지 검토했습니다.

## 2. ISO 19157 표준 구조

ISO 19157은 다음으로 구성됩니다:
- **ISO 19157-1:2023**: Geographic information — Data quality — Part 1: General requirements
- **ISO 19157-2:2016**: Geographic information — Data quality — Part 2: XML schema implementation
- **ISO 19157-3:2025**: Geographic information — Data quality — Part 3: Quality measures registry (최신 버전)

**중요**: ISO 19157-3:2025는 ISO 19135-1:2015에 따라 레지스터 기반 ID 관리 체계를 제공합니다.

## 3. 검토 결과

### 3.1 명시적 규정 없음

**결론: ISO 19157 표준은 RuleID나 측정ID 부여 방식에 대한 구체적인 규정이나 규칙을 명시하지 않습니다.**

주요 발견 사항:
1. **품질 요소 정의**: ISO 19157은 데이터 품질 요소(Quality Element)와 세부 항목(Sub-element)을 정의하지만, 이들에 대한 식별자 부여 방식을 규정하지 않습니다.
2. **측정 방법 정의**: 데이터 품질 측정 방법(Data Quality Measure)을 정의하지만, 측정 방법에 대한 식별자 명명 규칙은 제공하지 않습니다.
3. **메타데이터 구조**: ISO 19115 (메타데이터 표준)과 연계하여 품질 메타데이터 구조를 정의하지만, 식별자 형식에 대한 규정은 없습니다.

### 3.2 ISO 19157-3:2025 Quality Measures Registry (수정)

**중요 발견**: ISO 19157-3:2025는 ISO 19135-1:2015에 따라 레지스터 기반 ID 관리 체계를 제공합니다.

ISO 19157-3:2025의 주요 내용:
- **ISO 19135-1:2015 준수**: 등록 항목(Registration Item)의 식별자 부여 방식을 ISO 19135-1에 따라 정의
- **레지스터 구조**: 데이터 품질 측정 방법의 등록 및 유지 관리 절차 정의
- **등록 기관**: OGC(Open Geospatial Consortium)가 등록 기관으로 지정됨 (2022년)
- **기계 판독 가능**: XML 및 지리공간 API 형식으로 레지스터 구현 및 접근

**ISO 19135-1:2015의 식별자 구조**:
- 등록 기관 코드(Registering Authority Code)
- 등록 항목 식별자(Item Identifier)
- 버전 번호(Version Number)

이를 통해 ISO 19157-3:2025는 레지스터 기반의 체계적인 ID 관리 방식을 제공합니다.

### 3.3 ISO 19115 메타데이터 표준과의 관계

ISO 19115 (Geographic information — Metadata)에서:
- 데이터 품질 정보를 메타데이터로 표현하는 구조를 정의합니다.
- `DQ_Measure` 클래스를 통해 품질 측정 방법을 설명합니다.
- 하지만 `DQ_Measure`의 식별자 필드에 대한 명명 규칙은 제공하지 않습니다.

## 4. ISO 19157 표준이 제공하는 것

### 4.1 품질 요소 및 세부 항목 분류

ISO 19157은 다음을 제공합니다:
- **5가지 품질 요소**: Completeness, Logical Consistency, Positional Accuracy, Temporal Quality, Thematic Quality
- **세부 항목**: 각 품질 요소별 세부 항목 (예: Omission, Commission, Topological Consistency 등)
- **측정 방법 개념**: 각 세부 항목에 대한 측정 방법의 개념적 정의

### 4.2 측정 방법 메타데이터 구조

ISO 19157-3:2025에서 정의하는 측정 방법 메타데이터 구조 (ISO 19135-1:2015 기반):
- **등록 기관 코드(Registering Authority Code)**: 등록 기관 식별 (예: OGC)
- **등록 항목 식별자(Item Identifier)**: 등록 항목의 고유 식별자
- **버전 번호(Version Number)**: 등록 항목의 버전
- 측정 방법 이름(Name)
- 측정 방법 설명(Description)
- 측정 방법 정의(Definition)
- 측정 방법 식별자(Identifier) - **ISO 19135-1에 따른 구조 사용**

**ISO 19135-1:2015 식별자 형식 예시**:
- `{RegisteringAuthorityCode}:{ItemIdentifier}:{VersionNumber}`
- 예: `OGC:DQM-001:1.0` (OGC 등록 기관, 항목 ID, 버전)

## 5. ISO 19135-1:2015 기반 식별자 구조

### 5.1 ISO 19135-1:2015 등록 항목 식별자 구조

ISO 19135-1:2015는 등록 항목에 대한 식별자 구조를 정의합니다:

**구조**: `{RegisteringAuthorityCode}:{ItemIdentifier}:{VersionNumber}`

**구성 요소**:
- **Registering Authority Code**: 등록 기관 코드 (예: OGC, 국가 코드 등)
- **Item Identifier**: 등록 항목의 고유 식별자 (조직이 정의)
- **Version Number**: 버전 번호 (선택사항)

**예시**:
- `OGC:DQM-001:1.0`: OGC 등록 기관의 데이터 품질 측정 방법
- `KR-NGII:DQM-TC-DP-001:1.0`: 한국 국토지리정보원 등록 기관의 위상 일관성 중복 검사

### 5.2 ISO 19157-3:2025 레지스터 활용

ISO 19157-3:2025 레지스터를 활용하는 경우:
1. **OGC 레지스터 등록**: OGC가 등록 기관이므로 OGC 레지스터에 등록
2. **ISO 19135-1 구조 준수**: 등록 항목 식별자는 ISO 19135-1 구조 사용
3. **메타데이터 제공**: 각 등록 항목에 대한 완전한 메타데이터 제공

## 6. 권장 사항

### 6.1 조직 자체 규칙 수립

ISO 19157-3:2025는 ISO 19135-1:2015에 따른 레지스터 기반 ID 관리를 제공하지만:
1. **조직의 필요에 맞는 RuleID 체계 설계**: ISO 19157의 품질 요소와 세부 항목을 참고하여 자체 식별자 체계를 구축
2. **ISO 19135-1 구조 고려**: 레지스터 등록을 고려한다면 ISO 19135-1 구조 준수 검토
3. **일관성 유지**: 조직 내에서 일관된 명명 규칙 적용
4. **표준 준수**: ISO 19157의 품질 요소 분류를 RuleID 구조에 반영

### 6.2 ISO 19157 기반 RuleID 설계 원칙

제안된 RuleID 구조가 ISO 19157 표준을 준수하는 방법:

1. **품질 요소 반영**: RuleID에 ISO 19157 품질 요소를 반영
   - 예: `3-TC-DP-001` (위상 일관성 - Topological Consistency)

2. **세부 항목 반영**: ISO 19157 세부 항목을 RuleID에 포함
   - 예: `3-RP-SH-001` (상대 위치 정확도 - Relative Positional Accuracy)

3. **메타데이터 연계**: RuleID와 함께 ISO 19157 메타데이터 구조로 품질 정보 관리
   - `DQ_Measure` 메타데이터에 RuleID를 식별자로 사용

4. **ISO 19135-1 구조 고려** (선택사항):
   - 레지스터 등록을 고려하는 경우: `{RegisteringAuthorityCode}:{RuleID}:{Version}`
   - 예: `KR-NGII:3-TC-DP-001:1.0`
   - 내부 사용만 하는 경우: 현재 구조 유지 가능

### 5.3 국제 표준 준수 예시

다른 국가나 조직의 사례:
- **INSPIRE**: 유럽의 공간정보 인프라에서 자체 식별자 체계 사용
- **OGC**: Open Geospatial Consortium에서도 자체 식별자 체계 정의
- **국가기본도**: 각 국가가 자체 RuleID 체계 구축

## 7. 결론

### 7.1 핵심 발견 (수정)

1. **ISO 19157-3:2025는 ISO 19135-1:2015에 따른 레지스터 기반 ID 관리 체계를 제공**
2. **ISO 19135-1:2015는 등록 항목 식별자 구조를 정의** (`{RegisteringAuthorityCode}:{ItemIdentifier}:{VersionNumber}`)
3. **레지스터 등록을 하지 않는 경우, 조직의 자율적 설계 영역**
4. **표준은 품질 요소와 세부 항목의 개념적 분류를 제공**

### 7.2 제안된 RuleID 구조의 타당성

현재 제안된 RuleID 구조 (`[STAGE][ISO_SUB_ELEMENT][TYPE][SEQUENCE]`)는:
- ✅ ISO 19157 표준의 품질 요소와 세부 항목을 반영
- ✅ 표준의 요구사항을 위반하지 않음
- ✅ 조직의 필요에 맞게 설계됨
- ✅ 표준 준수 및 확장성 확보
- ✅ ISO 19135-1 구조와 호환 가능 (레지스터 등록 시 접두사 추가 가능)

### 7.3 권장 사항 (수정)

**옵션 1: 내부 사용만 하는 경우**
1. **현재 제안된 RuleID 구조 유지**: `3-TC-DP-001` 형식
2. **메타데이터 문서화**: 각 RuleID에 대한 ISO 19157 메타데이터 작성
3. **표준 준수 확인**: RuleID가 ISO 19157 품질 요소와 올바르게 매핑되는지 검증

**옵션 2: 레지스터 등록을 고려하는 경우**
1. **ISO 19135-1 구조 준수**: `{RegisteringAuthorityCode}:{RuleID}:{Version}` 형식
2. **예시**: `KR-NGII:3-TC-DP-001:1.0`
3. **OGC 레지스터 등록 검토**: 필요 시 OGC 레지스터에 등록
4. **메타데이터 완전성**: ISO 19157-3:2025 요구사항에 맞는 완전한 메타데이터 제공

## 8. 참고 자료

- ISO 19157-1:2023 Geographic information — Data quality — Part 1: General requirements
- ISO 19157-2:2016 Geographic information — Data quality — Part 2: XML schema implementation
- ISO 19157-3:2025 Geographic information — Data quality — Part 3: Quality measures registry
- ISO 19135-1:2015 Geographic information — Procedures for item registration — Part 1: Fundamentals
- ISO 19115-1:2014 Geographic information — Metadata — Part 1: Fundamentals

## 9. 추가 고려사항

### 9.1 ISO 19135-1:2015 등록 기관 코드

등록 기관 코드 예시:
- `OGC`: Open Geospatial Consortium
- `KR-NGII`: 한국 국토지리정보원
- `US-NGA`: 미국 국립지리정보원
- `EU-INSPIRE`: 유럽 INSPIRE

### 9.2 레지스터 등록의 장단점

**장점**:
- 국제 표준 준수
- 다른 조직과의 상호 운용성
- 표준화된 메타데이터 관리

**단점**:
- 등록 절차 및 유지 관리 필요
- 식별자 길이 증가
- 내부 사용만 하는 경우 과도할 수 있음

---

**검토일**: 2025-11-18 (수정: ISO 19157-3:2025 레지스터 기반 ID 관리 추가)  
**검토자**: SpatialCheckPro 개발팀  
**결론**: ISO 19157-3:2025는 ISO 19135-1:2015에 따른 레지스터 기반 ID 관리 체계를 제공합니다. 레지스터 등록을 고려하는 경우 ISO 19135-1 구조를 준수하고, 내부 사용만 하는 경우 현재 제안된 구조를 유지할 수 있습니다.

