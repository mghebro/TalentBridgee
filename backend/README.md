# Full Stack საფინალო პროექტის დოკუმენტაცია

## სტუდენტის ინფორმაცია
* **სახელი, გვარი:** გიორგი მღებრიშვილი
* **ჯგუფი:** 
* **თარიღი:** _______________________
* **საკონტაქტო ელ-ფოსტა:** mghebrom@gmail.com
---

## 1. პროექტის ზოგადი აღწერა

### 1.1 პროექტის დასახელება
**TalentBridge** - უნივერსალური HR მენეჯმენტისა და რეკრუტირების სისტემა

### 1.2 პროექტის მოკლე აღწერა
TalentBridge არის ვებ-პლატფორმა, რომელიც საშუალებას აძლევს ყველა ტიპის ორგანიზაციას (სკოლები, კლინიკები, კომპანიები, სახელმწიფო უწყებები) ეფექტურად განახორციელონ პერსონალის შერჩევის პროცესი. სისტემა მოიცავს ვაკანსიების გამოქვეყნებას, კანდიდატების რეგისტრაციას, პროფესიულ ონლაინ ტესტირებას 6 ტიპის კითხვებით და ავტომატურ შედეგების შეფასებას ანალიტიკით.

### 1.3 რეალური პრობლემა, რომელსაც პროექტი აგვარებს

**პრობლემის დეტალური აღწერა:**
ორგანიზაციები აწყდებიან გამოწვევებს თანამშრომელთა შერჩევის პროცესში - ტრადიციული მეთოდები არის დროში ხანგრძლივი, ძვირი და რესურსმოთხოვნადი. ფიზიკური ინტერვიუები და ქაღალდზე დაფუძნებული ტესტები ზღუდავს კანდიდატების რაოდენობას და ობიექტური შეფასების შესაძლებლობას. სტანდარტიზებული ტესტირების არარსებობა და დიდი განმცხადებელთა ნაკადის არაეფექტური მართვა ქმნის ბარიერებს დასაქმებაში.

**ვის ეხმარება თქვენი პროექტი:**
- HR მენეჯერები ყველა ინდუსტრიიდან
- სამუშაოს მაძიებლები ყველა პროფესიიდან
- მცირე და საშუალო ბიზნესები HR განყოფილების გარეშე
- სახელმწიფო უწყებები გამჭვირვალე დასაქმების პროცესისთვის
- საგანმანათლებლო დაწესებულებები მასწავლებელთა შერჩევისთვის
- სამედიცინო დაწესებულებები მედპერსონალის დასაქმებისთვის

**რატომ არის ეს პრობლემა მნიშვნელოვანი:**
სწორი პერსონალის შერჩევა კრიტიკულია ნებისმიერი ორგანიზაციის წარმატებისთვის. ცუდი გადაწყვეტილებები იწვევს დროისა და ფულის ხარჯვას თანამშრომელთა ცვლის, დაბალი პროდუქტიულობისა და ტრენინგის ხარჯების გამო. ეფექტური HR სისტემა ზოგავს დროს (დასაქმების დრო მცირდება 50%-ით), ამცირებს ხარჯებს (30-40%-ით) და უზრუნველყოფს საუკეთესო კანდიდატების მოძიებას ობიექტური კრიტერიუმებით.

---

## 2. ტექნიკური სტეკი

### 2.1 Backend ტექნოლოგიები
* **Framework:** ASP.NET Web API (.NET ვერსია: 8.0)
* **მონაცემთა ბაზა:** ☑ SQL Server
* **ORM:** ☑ Entity Framework Core
* **Authentication:** JWT (JSON Web Tokens)
* **Email Service:** SendGrid / SMTP
* **File Storage:** Azure Blob Storage / Local File System
* **დამატებითი ბიბლიოთეკები:**
  - AutoMapper (DTO mapping)
  - FluentValidation (input validation)
  - Serilog (logging)
  - Swashbuckle (API documentation)
  - BCrypt.Net (password hashing)

### 2.2 Frontend ტექნოლოგიები
* **ძირითადი Framework:** ☑ Angular (ვერსია: 17)
* **CSS Framework:** ☑ Bootstrap (ვერსია 5)
* **დამატებითი ბიბლიოთეკები:**
  - Angular Material (UI components)
  - Chart.js (data visualization)
  - ng-bootstrap
  - RxJS (reactive programming)
  - PrimeNG

---

## 3. API მოდელები და სტრუქტურა

### 3.1 ძირითადი მოდელები

**მოდელი 1: User**
```
- int Id (Primary Key)
- string Email (unique, required)
- string Password (hashed)
- ROLES Role (enum: USER, HR_MANAGER, ADMIN)
- bool IsVerified
- string FirstName, LastName
- DateTime CreatedAt, UpdatedAt
Navigation: UserDetails (1:1), Applications (1:N), Notifications (1:N)
```

**მოდელი 2: Organization**
```
- int Id (Primary Key)
- string Name, Address, ContactEmail, PhoneNumber
- TYPES Type (enum: School, Clinic, Company, Government, NGO)
- string Description, Logo, Website
- DateTime CreatedAt, UpdatedAt
Navigation: HRManagers (1:N), Vacancies (1:N), Tests (1:N)
```

**მოდელი 3: Vacancy**
```
- int Id (Primary Key)
- int OrganizationId, CreatedBy
- string Title, Description, Requirements, Responsibilities
- string Profession, Industry, Location
- EMPLOYMENT_TYPE EmploymentType
- decimal SalaryMin, SalaryMax
- VACANCY_STATUS Status
- DateTime ApplicationDeadline, PublishedAt
Navigation: Applications (1:N), Organization (N:1)
```

**მოდელი 4: Application**
```
- int Id (Primary Key)
- int VacancyId, UserId
- APPLICATION_STATUS Status (default: Pending)
- string CoverLetter, ReviewNotes
- DateTime AppliedAt
Navigation: Vacancy (N:1), User (N:1), TestAssignments (1:N)
```

**მოდელი 5: Test**
```
- int Id (Primary Key)
- int OrganizationId, CreatedBy
- string Title, Description, Profession
- int DurationMinutes (5-300)
- decimal PassingScore, TotalPoints
- TEST_DIFFICULTY Difficulty
Navigation: Questions (1:N), Assignments (1:N)
```

**მოდელი 6: Question**
```
- int Id (Primary Key)
- int TestId
- string QuestionText
- QUESTION_TYPE QuestionType (MultipleChoice, TrueFalse, ShortAnswer, Essay, Coding, FileUpload)
- decimal Points
- int OrderNumber
Navigation: Test (N:1), Options (1:N), Answers (1:N)
```

**მოდელი 7: TestSubmission**
```
- int Id (Primary Key)
- int TestAssignmentId, UserId, TestId
- DateTime StartTime, EndTime, SubmittedAt
- decimal TotalPointsEarned, PercentageScore
- bool IsPassed
- string Feedback
Navigation: Answers (1:N), User (N:1)
```

**დამატებითი მოდელები:** UserDetails, HRManager, QuestionOption, TestAssignment, SubmissionAnswer, Notification, Message, SavedVacancy

### 3.2 მოდელებს შორის კავშირები

**One-to-One:**
- User ↔ UserDetails
- User ↔ HRManager
- TestAssignment ↔ TestSubmission

**One-to-Many:**
- User → Applications, Notifications, Messages
- Organization → Vacancies, Tests, HRManagers
- Vacancy → Applications, SavedVacancies
- Test → Questions, Assignments
- Application → TestAssignments

**Many-to-Many:**
- Users ↔ Vacancies (via SavedVacancy)
- Applications ↔ Tests (via TestAssignment)

### 3.3 API Endpoints (70+)

| HTTP Method | Endpoint | აღწერა |
|-------------|----------|--------|
| POST | /api/auth/register | მომხმარებლის რეგისტრაცია |
| POST | /api/auth/login | ავტორიზაცია და JWT გენერაცია |
| POST | /api/auth/refresh-token | Token-ის განახლება |
| GET | /api/users/me | მომხმარებლის პროფილი |
| PUT | /api/users/me/details | პროფილის განახლება |
| POST | /api/users/me/cv | CV-ის ატვირთვა |
| POST | /api/organizations | ორგანიზაციის შექმნა |
| GET | /api/organizations | ორგანიზაციების სია |
| POST | /api/vacancies | ვაკანსიის შექმნა |
| GET | /api/vacancies | ვაკანსიების სია ფილტრებით |
| GET | /api/vacancies/{id} | ვაკანსიის დეტალები |
| POST | /api/applications | განაცხადის გაგზავნა |
| GET | /api/applications/my-applications | ჩემი განაცხადები |
| PATCH | /api/applications/{id}/status | სტატუსის შეცვლა |
| POST | /api/tests | ტესტის შექმნა |
| POST | /api/tests/{id}/questions | კითხვის დამატება |
| POST | /api/test-assignments | ტესტის მინიჭება |
| POST | /api/test-submissions/{id}/start | ტესტის დაწყება |
| POST | /api/test-submissions/{id}/submit | ტესტის გაგზავნა |
| GET | /api/test-submissions/{id}/results | ტესტის შედეგები |
| POST | /api/saved-vacancies | ვაკანსიის შენახვა |
| GET | /api/notifications | შეტყობინებები |
| POST | /api/messages | შეტყობინების გაგზავნა |
| GET | /api/dashboard/candidate | კანდიდატის დაფა |
| GET | /api/dashboard/hr-manager | HR დაფა |

---

## 4. ფუნქციონალი

### 4.1 ძირითადი ფუნქციები

1. **მრავალროლიანი ავტენტიფიკაცია** - Email/password და OAuth (Google, Apple), JWT tokens, email verification
2. **სრული პროფილის მართვა** - პირადი ინფო, განათლება, გამოცდილება, უნარები, CV ატვირთვა
3. **ორგანიზაციების მართვა** - 5 ტიპი (School, Clinic, Company, Government, NGO), ბრენდინგი, მრავალი HR მენეჯერი
4. **ვაკანსიების მართვა** - დეტალური აღწერა, მოთხოვნები, ხელფასის დიაპაზონი, დედლაინები, სტატუსები
5. **განაცხადების ტრეკინგი** - 9 სტატუსი, cover letter, timeline, HR შენიშვნები
6. **მოქნილი ტესტირება** - 6 ტიპის კითხვები, ავტომატური და manual შეფასება, დროის ლიმიტი
7. **უსაფრთხო ტესტის გავლა** - უნიკალური access tokens, auto-save, დროის ტრეკინგი
8. **ინტელექტუალური შეფასება** - ავტოგრადირება + manual review, feedback სისტემა
9. **შიდა კომუნიკაცია** - პირდაპირი შეტყობინებები, threading, განაცხადთან დაკავშირებული საუბრები
10. **შეტყობინებების სისტემა** - 10+ ტიპი, real-time updates, deep linking
11. **ძიებისა და აღმოჩენის სისტემა** - მრავალკრიტერიუმიანი ფილტრები, keyword search, შენახული ვაკანსიები
12. **ანალიტიკა და რეპორტები** - dashboards, სტატისტიკა, ექსპორტი CSV/PDF
13. **ფაილების მართვა** - CV, სურათები, ლოგოები, ტესტის ფაილები, უსაფრთხო შენახვა
14. **აუდიტი და უსაფრთხოება** - სრული audit trail, GDPR compliance, სტატუსის ისტორია
15. **საჯარო Job Board** - საჯარო ძიება ავტორიზაციის გარეშე, ორგანიზაციების პროფილები

### 4.2 მომხმარებლის როლები

**ადმინისტრატორი (ADMIN)**
- უფლებები: სრული სისტემის წვდომა, ყველა ორგანიზაციის მართვა, system-wide analytics, audit logs, მომხმარებლების მართვა

**HR მენეჯერი (HR_MANAGER)**
- უფლებები: ვაკანსიების შექმნა/მართვა, განაცხადების განხილვა, ტესტების შექმნა/მინიჭება, manual grading, კანდიდატებთან კომუნიკაცია, ორგანიზაციის ანალიტიკა

**რეგისტრირებული მომხმარებელი (USER/Candidate)**
- უფლებები: ვაკანსიების ძიება, განაცხადების გაგზავნა, პროფილის მართვა (განათლება, გამოცდილება, CV), ტესტების გავლა, შეტყობინებები, შენახული ვაკანსიები, dashboard

**სტუმარი (Guest)**
- უფლებები: საჯარო ვაკანსიების ნახვა, ძიება ფილტრებით, ორგანიზაციების პროფილების ნახვა, რეგისტრაცია

### 4.3 ავტორიზაციის ლოგიკა

- **JWT Token-Based:** Access tokens (15-30 წთ) + Refresh tokens (7-30 დღე)
- **Role-Based Access Control (RBAC):** `[Authorize]` attributes, role-specific endpoints
- **Security Checks:** Email verification, password strength, rate limiting, account lockout, unique test tokens
- **Resource Ownership:** მომხმარებლები მხოლოდ საკუთარ მონაცემებს მართავენ, HR მენეჯერები მხოლოდ საკუთარ ორგანიზაციას

---

## 5. მონაცემთა ბაზის სქემა

### 5.1 ცხრილები და მათი დანიშნულება

1. **Users** - მომხმარებლის ანგარიშები (email, password, role)
2. **UserDetails** - დეტალური პროფილი (ტელეფონი, bio, CV, სურათი)
3. **Organizations** - ორგანიზაციების ინფო (სახელი, ტიპი, კონტაქტები)
4. **HRManagers** - HR მენეჯერები (მომხმარებელი + ორგანიზაცია)
5. **Vacancies** - ვაკანსიები (აღწერა, მოთხოვნები, ხელფასი, სტატუსი)
6. **Applications** - განაცხადები (კანდიდატი + ვაკანსია, სტატუსი)
7. **Tests** - ტესტები (სათაური, ხანგრძლივობა, passing score)
8. **Questions** - კითხვები (ტექსტი, ტიპი, ქულები)
9. **QuestionOptions** - პასუხის ვარიანტები (ტექსტი, სწორია თუ არა)
10. **TestAssignments** - ტესტის მინიჭება (განაცხადი + ტესტი)
11. **TestSubmissions** - გავლილი ტესტები (ქულები, pass/fail)
12. **SubmissionAnswers** - ინდივიდუალური პასუხები
13. **Notifications** - შეტყობინებები (ტიპი, წაკითხული თუ არა)
14. **Messages** - შიდა შეტყობინებები
15. **AuditLogs** - audit trail (ყველა ოპერაცია)

### 5.2 ინდექსები და ოპტიმიზაცია

**Users:** Email (Unique), Role, IsVerified  
**Applications:** (VacancyId, UserId) Composite, (Status, AppliedAt)  
**Vacancies:** (Status, PublishedAt), (Profession, Industry), ApplicationDeadline  
**TestSubmissions:** TestAssignmentId (Unique), (UserId, TestId)  
**Notifications:** (UserId, IsRead, CreatedAt)  
**Messages:** (SenderId, ReceiverId), (ReceiverId, IsRead)  

**Optimization:**
- Eager Loading (.Include)
- Pagination (default 10, max 100)
- Async operations
- Query Projections (DTOs)
- Redis caching
- Connection pooling

---

## 6. დამატებითი ფუნქციონალი

### 6.1 ინტეგრაციები

**Email Service (SendGrid/SMTP)**
- Verification emails, password reset, notifications, test assignments, status updates

**File Storage (Azure Blob/AWS S3)**
- CV/resume (max 10MB), profile pictures (max 5MB), org logos (max 2MB), test files (max 25MB)
- Features: CDN, signed URLs, auto-cleanup

**OAuth Providers**
- Google OAuth 2.0, Apple Sign In

**PDF Generation (Future)**
- Test reports, application summaries, analytics reports

---

## 9. პროექტის უნიკალური მახასიათებლები

### 9.1 რით განსხვავდება თქვენი პროექტი მსგავსი გადაწყვეტილებებისგან?

1. **უნივერსალური მულტი-ინდუსტრიული პლატფორმა** - ემსახურება ყველა ინდუსტრიას თანაბრად (განათლება, ჯანდაცვა, IT, მთავრობა)
2. **ყოვლისმომცველი ტესტირება** - 6 ტიპის კითხვა (MultipleChoice, Essay, Coding და სხვა) + hybrid grading
3. **სრული Audit Trail** - ApplicationTimeline, AuditLog, VacancyView ტრეკინგი
4. **ქართული ბაზრის ფოკუსი** - GEL currency, ადგილობრივი ინდუსტრიების გაგება, მომავალში ქართული ლოკალიზაცია
5. **დამატებითი პროფილის მართვა** - ცალკე Education და Experience entities, skills JSON array
6. **ინტეგრირებული კომუნიკაცია** - built-in messaging + threading, 10+ notification types
7. **უსაფრთხო ტესტირება** - უნიკალური access tokens, auto-save, time tracking
8. **მოქნილი ორგანიზაციული მოდელი** - Generic `Organization<TExactType>`, multiple HR managers
9. **Smart Status Workflows** - 9 application statuses, vacancy lifecycle
10. **Analytics-Driven** - vacancy performance, test analytics, time-to-hire, exportable reports

### 9.2 ინოვაციური ფუნქციები

**მიმდინარე:**
- Hybrid grading system (auto + manual)
- Per-question time limits
- Reusable test library
- Complete transparency with audit trails

**მომავალი:**
- AI-Powered Candidate Ranking (ML algorithms)
- Smart Matching (automatic candidate-vacancy matching)
- Video Interview Integration
- Blockchain Credential Verification
- Adaptive Testing (difficulty adjusts dynamically)
- Gamification (points, badges, achievements)

---

## 10. განვითარების გეგმა

### 10.2 პოტენციური გამოწვევები და რისკები

1. **Test Security** - Cheating prevention → Mitigation: unique tokens, time limits, question randomization
2. **Scalability** - Large organizations → Mitigation: indexing, pagination, caching (Redis), microservices
3. **GDPR Compliance** - Sensitive data → Mitigation: encryption, audit logs, data export/deletion
4. **File Storage Costs** - Thousands of files → Mitigation: size limits, compression, lifecycle policies
5. **Email Deliverability** - Spam folders → Mitigation: SendGrid, SPF/DKIM/DMARC
6. **Manual Grading Bottlenecks** → Mitigation: clear UI, bulk operations, AI-assisted grading
7. **Browser Compatibility** → Mitigation: Angular browser support, polyfills
8. **Mobile Responsiveness** → Mitigation: Bootstrap responsive grid, mobile-first design

### 10.3 მომავალი განვითარება

**მოკლევადიანი (3-6 თვე):**
- Mobile apps (iOS, Android)
- Elasticsearch integration
- Video interviews (Zoom/Teams API)
- Calendar integration
- Multi-language support (ქართული, ინგლისური)

**საშუალოვადიანი (6-12 თვე):**
- AI Candidate Matching (ML)
- Automated screening
- Chatbot assistant
- Blockchain verification
- Adaptive testing

**გრძელვადიანი (12+ თვე):**
- Marketplace (third-party integrations)
- Public API
- White-label solution
- Predictive analytics
- Global expansion (multi-currency, timezones)

---

## 11. დამატებითი შენიშვნები

**პროექტის ხედვა:** TalentBridge-ის მიზანია გახდეს წამყვანი HR პლატფორმა საქართველოში, მომავალში გაფართოვდეს მეზობელ ქვეყნებსა და გლობალურ ბაზრებზე.

**წარმატების კრიტერიუმები:**
- ტექნიკური: 10,000+ concurrent users, API response < 500ms, 99.9% uptime
- ბიზნეს: 100+ organizations, 10,000+ candidates in 6 months
- User Satisfaction: NPS > 50, rating > 4.5/5

**სოციალური გავლენა:**
- უკეთესი job matching-ით ზრდის დასაქმების შესაძლებლობებს
- ობიექტური ტესტირებით ამცირებს დასაქმების bias-ს
- მცირე ბიზნესებს აძლევს ხელმისაწვდომ HR tools-ს
- უზრუნველყოფს გამჭვირვალობას სახელმწიფო დასაქმებაში

**Deployment:**
- Development: Local SQL Server
- Production: Azure App Service + Azure SQL Database (or AWS)
- CI/CD: GitHub Actions / Azure DevOps

---

**პროექტის სტატუსი:** In Development  
**დოკუმენტის ვერსია:** 1.0
