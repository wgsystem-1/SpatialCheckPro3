# ISO 19157-1:2023 품질요소 Abbreviation 체계 및 RuleID 합성 방안

## 1. ISO 19157-1:2023 품질요소 구조화

### 1.1 품질요소 5가지 및 세부 항목

| 품질요소 (Quality Element) | Abbreviation | 세부 항목 (Sub-element) | Abbreviation | 설명 |
|---------------------------|--------------|------------------------|--------------|------|
| **1. Completeness (완전성)** | `C` | Commission (초과) | `COM` | 존재하면 안 되는 항목 포함 정도 |
| | | Omission (누락) | `OMI` | 존재해야 하는 항목 누락 정도 |
| **2. Logical Consistency (논리 일관성)** | `L` | Conceptual Consistency (개념 일관성) | `CC` | 개념 스키마 규칙 준수 여부 |
| | | Domain Consistency (도메인 일관성) | `DC` | 값 영역·코딩 규칙 부합 여부 |
| | | Format Consistency (형식 일관성) | `FC` | 물리적 구조·포맷 제약 준수 여부 |
| | | Topological Consistency (위상 일관성) | `TC` | 위상 관계 정확성 (접합, 겹침, 갭, 자기교차 등) |
| **3. Positional Accuracy (위치 정확도)** | `P` | Absolute/External Positional Accuracy (절대 위치 정확도) | `APA` | 좌표값이 참값과의 근접도 (CE95 등) |
| | | Relative/Internal Positional Accuracy (상대 위치 정확도) | `RPA` | 동일 데이터셋 내 객체 간 상대 위치 정확성 |
| | | Gridded Data Positional Accuracy (격자 데이터 위치 정확도) | `GPA` | 격자 데이터 셀의 공간 위치 일치도 |
| **4. Temporal Quality (시간 품질)** | `T` | Accuracy of a Time Measurement (시간 측정 정확도) | `ATM` | 타임스탬프가 참값과 근접한 정도 |
| | | Temporal Consistency (시간 일관성) | `TC` | 이벤트 순서/시간 관계의 논리적 정합성 |
| | | Temporal Validity (시간 유효성) | `TV` | 주어진 시간영역 내 데이터 유효성 |
| **5. Thematic Quality (주제 품질)** | `Q` | Classification Correctness (분류 정확성) | `CC` | 피처의 분류(클래스) 정확성 |
| | | Non-quantitative Attribute Correctness (비정량 속성 정확성) | `NAC` | 비정량 속성(문자/범주형) 값의 정확성 |
| | | Quantitative Attribute Accuracy (정량 속성 정확도) | `QAA` | 정량 속성(수치형) 값이 참값과 근접한 정도 |

### 1.2 Abbreviation 충돌 해결

**충돌 항목:**
- `TC`: Topological Consistency (논리 일관성) vs Temporal Consistency (시간 품질)
- `CC`: Conceptual Consistency (논리 일관성) vs Classification Correctness (주제 품질)

**해결 방안:**
- 품질요소 코드를 포함하여 구분: `L-TC` (논리 일관성-위상 일관성), `T-TC` (시간 품질-시간 일관성)
- 또는 컨텍스트에 따라 우선순위 부여 (더 자주 사용되는 것에 단축형 할당)

## 2. RuleID 합성 구조 제안

### 2.1 구조 1: 품질요소 + 세부항목 + 검수단계 + 유형 + 일련번호

```
[QUALITY_ELEMENT][SUB_ELEMENT][STAGE][TYPE][SEQUENCE]
```

**예시:**
- `C-OMI-1-TB-001`: 완전성-누락-1단계-테이블-001
- `L-TC-3-DP-001`: 논리 일관성-위상 일관성-3단계-중복-001
- `P-RPA-3-SH-001`: 위치 정확도-상대 위치 정확도-3단계-짧은객체-001
- `Q-CC-4-CD-001`: 주제 품질-분류 정확성-4단계-코드리스트-001

**장점:**
- ISO 19157 표준과 직접 매핑
- 품질요소 기반 통계 집계 용이
- 표준 준수 명확

**단점:**
- RuleID 길이가 길어짐 (약 15자)
- 기존 RuleID와 호환성 낮음

### 2.2 구조 2: 검수단계 + 품질요소 + 세부항목 + 유형 + 일련번호 (권장)

```
[STAGE][QUALITY_ELEMENT][SUB_ELEMENT][TYPE][SEQUENCE]
```

**예시:**
- `1-C-OMI-TB-001`: 1단계-완전성-누락-테이블-001
- `3-L-TC-DP-001`: 3단계-논리 일관성-위상 일관성-중복-001
- `3-P-RPA-SH-001`: 3단계-위치 정확도-상대 위치 정확도-짧은객체-001
- `4-Q-CC-CD-001`: 4단계-주제 품질-분류 정확성-코드리스트-001

**장점:**
- 검수 단계 우선으로 직관적
- 기존 구조와 유사하여 마이그레이션 용이
- 품질요소 기반 분류 가능

**단점:**
- RuleID 길이 증가 (약 13자)

### 2.3 구조 3: 검수단계 + 품질요소(단일문자) + 세부항목(단축) + 유형 + 일련번호 (최적화)

```
[STAGE][Q][SUB][TYPE][SEQUENCE]
```

**Abbreviation 단축표:**

| 품질요소 | Q | 세부 항목 | SUB | 설명 |
|---------|---|----------|-----|------|
| Completeness | C | Commission | COM → `CM` | 초과 |
| | | Omission | OMI → `OM` | 누락 |
| Logical Consistency | L | Conceptual Consistency | CC → `CC` | 개념 일관성 |
| | | Domain Consistency | DC → `DC` | 도메인 일관성 |
| | | Format Consistency | FC → `FC` | 형식 일관성 |
| | | Topological Consistency | TC → `TC` | 위상 일관성 |
| Positional Accuracy | P | Absolute Positional Accuracy | APA → `AP` | 절대 위치 정확도 |
| | | Relative Positional Accuracy | RPA → `RP` | 상대 위치 정확도 |
| | | Gridded Positional Accuracy | GPA → `GP` | 격자 위치 정확도 |
| Temporal Quality | T | Accuracy of Time Measurement | ATM → `AM` | 시간 측정 정확도 |
| | | Temporal Consistency | TC → `TC` | 시간 일관성 |
| | | Temporal Validity | TV → `TV` | 시간 유효성 |
| Thematic Quality | Q | Classification Correctness | CC → `CC` | 분류 정확성 |
| | | Non-quantitative Attribute | NAC → `NA` | 비정량 속성 |
| | | Quantitative Attribute | QAA → `QA` | 정량 속성 |

**예시:**
- `1-C-OM-TB-001`: 1단계-완전성-누락-테이블-001
- `3-L-TC-DP-001`: 3단계-논리 일관성-위상 일관성-중복-001
- `3-P-RP-SH-001`: 3단계-위치 정확도-상대 위치 정확도-짧은객체-001
- `4-Q-CC-CD-001`: 4단계-주제 품질-분류 정확성-코드리스트-001

**장점:**
- RuleID 길이 최적화 (약 11자)
- ISO 19157 표준 반영
- 기존 구조와 호환 가능

**단점:**
- Abbreviation 암기 필요
- 일부 충돌 가능 (TC, CC 등)

### 2.4 구조 4: 하이브리드 (기존 구조 + 품질요소 접두사) - 최종 권장안

```
[STAGE][Q][TYPE][SEQUENCE]
```

**설명:**
- 기존 `[STAGE][CATEGORY][TYPE][SEQUENCE]` 구조 유지
- `CATEGORY`를 ISO 19157 세부 항목으로 매핑
- 품질요소는 RuleID 접두사로 표기 (선택사항) 또는 메타데이터로 관리

**매핑 규칙:**

| 검수 단계 | 기존 CATEGORY | ISO 19157 품질요소 | ISO 19157 세부 항목 | 새 CATEGORY |
|----------|--------------|-------------------|-------------------|------------|
| 1단계 | TB, FC, CR, GT | Completeness | Omission | `OM` |
| 2단계 | FD, DT, UK, FK, NN | Logical Consistency | Format Consistency | `FC` |
| | | | Domain Consistency | `DC` |
| 3단계 | VL, NU, EM, SM | Logical Consistency | Format Consistency | `FC` |
| | DP, OV, SI, SO, HO | Logical Consistency | Topological Consistency | `TC` |
| | SH, SA, SL, SP, US, OS | Positional Accuracy | Relative Positional Accuracy | `RP` |
| 4단계 | CD, RG, RN, IF | Thematic Quality | Classification Correctness | `CC` |
| | | | Non-quantitative Attribute | `NA` |
| | | | Quantitative Attribute | `QA` |
| 5단계 | PI, LW, PW, PN, LC | Logical Consistency | Topological Consistency | `TC` |
| | PS | Positional Accuracy | Relative Positional Accuracy | `RP` |

**예시:**
- `1-OM-TB-001`: 1단계-완전성(누락)-테이블-001
- `2-FC-FD-001`: 2단계-논리 일관성(형식 일관성)-필드정의-001
- `3-TC-DP-001`: 3단계-논리 일관성(위상 일관성)-중복-001
- `3-RP-SH-001`: 3단계-위치 정확도(상대 위치 정확도)-짧은객체-001
- `4-CC-CD-001`: 4단계-주제 품질(분류 정확성)-코드리스트-001
- `5-TC-PI-001`: 5단계-논리 일관성(위상 일관성)-점면포함-001

**장점:**
- 기존 구조와 호환성 높음
- RuleID 길이 유지 (약 9자)
- ISO 19157 표준 반영
- 마이그레이션 용이

**단점:**
- 품질요소가 RuleID에 직접 표시되지 않음 (메타데이터로 관리 필요)

## 3. 최종 권장 구조: 구조 4 (하이브리드) + 품질요소 메타데이터

### 3.1 RuleID 구조

```
[STAGE][ISO_SUB_ELEMENT][TYPE][SEQUENCE]
```

**길이:** 9자 (예: `3-TC-DP-001`)

### 3.2 품질요소 메타데이터 매핑

각 RuleID에 대한 품질요소 정보를 별도 테이블/CSV로 관리:

```csv
RuleId,QualityElement,SubElement,Stage,Category,Type,Description
3-TC-DP-001,Logical Consistency,Topological Consistency,3,TC,DP,객체 중복 검사
3-RP-SH-001,Positional Accuracy,Relative Positional Accuracy,3,RP,SH,짧은 객체 검사
4-CC-CD-001,Thematic Quality,Classification Correctness,4,CC,CD,코드리스트 검사
```

### 3.3 기존 RuleID와의 매핑

| 기존 RuleID | 새 RuleID | 품질요소 | 세부 항목 |
|------------|----------|---------|----------|
| `3DP001` | `3-TC-DP-001` | Logical Consistency | Topological Consistency |
| `3SH001` | `3-RP-SH-001` | Positional Accuracy | Relative Positional Accuracy |
| `4CD001` | `4-CC-CD-001` | Thematic Quality | Classification Correctness |

## 4. 구현 방안

### 4.1 CSV 파일 구조 개선

**3_geometry_check.csv 예시:**

```csv
RuleId,QualityElement,SubElement,Enabled,TableId,TableName,GeometryType,객체중복,객체간겹침,자체꼬임,Severity,Note
3-TC-DP-001,Logical Consistency,Topological Consistency,Y,tn_buld,건물,MULTIPOLYGON,Y,N,N,MAJOR,객체 중복 검사
3-TC-OV-001,Logical Consistency,Topological Consistency,Y,tn_buld,건물,MULTIPOLYGON,N,Y,N,MAJOR,객체 간 겹침 검사
3-RP-SH-001,Positional Accuracy,Relative Positional Accuracy,Y,tn_rodway_ctln,도로중심선,MULTILINESTRING,N,N,N,MAJOR,짧은 객체 검사
```

### 4.2 데이터베이스 스키마 확장

```sql
CREATE TABLE validation_rules (
    rule_id VARCHAR(15) PRIMARY KEY,
    quality_element VARCHAR(50) NOT NULL,  -- Completeness, Logical Consistency, etc.
    sub_element VARCHAR(50) NOT NULL,       -- Omission, Topological Consistency, etc.
    stage INTEGER NOT NULL,
    category VARCHAR(10) NOT NULL,          -- TC, RP, CC, etc.
    type VARCHAR(10) NOT NULL,             -- DP, SH, CD, etc.
    sequence INTEGER NOT NULL,
    description TEXT,
    enabled BOOLEAN DEFAULT TRUE,
    severity VARCHAR(10),
    created_date DATETIME,
    updated_date DATETIME
);

-- 인덱스 추가
CREATE INDEX idx_quality_element ON validation_rules(quality_element);
CREATE INDEX idx_sub_element ON validation_rules(sub_element);
CREATE INDEX idx_stage ON validation_rules(stage);
```

### 4.3 코드 상수 정의

```csharp
public static class CheckIds
{
    // ISO 19157 품질요소
    public const string QualityCompleteness = "Completeness";
    public const string QualityLogicalConsistency = "Logical Consistency";
    public const string QualityPositionalAccuracy = "Positional Accuracy";
    public const string QualityTemporalQuality = "Temporal Quality";
    public const string QualityThematicQuality = "Thematic Quality";
    
    // ISO 19157 세부 항목
    public const string SubOmission = "Omission";
    public const string SubTopologicalConsistency = "Topological Consistency";
    public const string SubRelativePositionalAccuracy = "Relative Positional Accuracy";
    public const string SubClassificationCorrectness = "Classification Correctness";
    
    // RuleID 예시
    public const string GeometryDuplicate = "3-TC-DP-001";
    public const string ShortObject = "3-RP-SH-001";
    public const string CodeListCheck = "4-CC-CD-001";
}
```

## 5. 마이그레이션 전략

### 5.1 단계별 전환

1. **Phase 1**: 기존 RuleID 유지 + 품질요소 메타데이터 추가
2. **Phase 2**: 새 RuleID 생성 + 기존 RuleID와 매핑 테이블 구축
3. **Phase 3**: 점진적 전환 (신규 규칙은 새 RuleID 사용)
4. **Phase 4**: 완전 전환 (기존 RuleID 제거)

### 5.2 호환성 유지

- `OriginalRuleId` 컬럼으로 기존 ID 보존
- 매핑 테이블로 양방향 변환 지원
- 기존 코드는 OriginalRuleId 참조, 신규 코드는 새 RuleID 사용

## 6. 장점 요약

1. **ISO 19157 표준 준수**: 품질요소와 세부 항목 명확히 반영
2. **통계 집계 용이**: 품질요소별/세부 항목별 오류 집계 가능
3. **표준 호환성**: 국제 표준과 직접 매핑
4. **확장성**: 새로운 품질요소 추가 용이
5. **가독성**: RuleID만으로 품질요소 파악 가능 (메타데이터 참조 시)

---

**작성일**: 2025-11-18  
**작성자**: SpatialCheckPro 개발팀  
**버전**: 1.0

