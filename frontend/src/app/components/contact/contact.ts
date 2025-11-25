import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzTypographyModule } from 'ng-zorro-antd/typography';

interface ContactInfo {
  icon: string;
  title: string;
  content: string;
  link?: string;
}

interface FAQ {
  question: string;
  answer: string;
}

@Component({
  selector: 'app-contact-us',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzCardModule,
    NzGridModule,
    NzIconModule,
    NzButtonModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzDividerModule,
    NzTypographyModule,
  ],
  templateUrl: './contact.html',
  styleUrls: ['./contact.scss'],
})
export class Contact {
  contactForm: FormGroup;
  isSubmitting = false;

  contactInfo: ContactInfo[] = [
    {
      icon: 'mail',
      title: 'Email',
      content: 'mghebrom@gmail.com',
      link: 'mailto:mghebrom@gmail.com',
    },
    {
      icon: 'phone',
      title: 'Phone',
      content: '+995 XXX XXX XXX',
      link: 'tel:+995XXXXXXXXX',
    },
    {
      icon: 'environment',
      title: 'Location',
      content: 'Tbilisi, Georgia',
    },
    {
      icon: 'clock-circle',
      title: 'Business Hours',
      content: 'Mon - Fri: 9:00 AM - 6:00 PM',
    },
  ];

  inquiryTypes = [
    { label: 'General Inquiry', value: 'general' },
    { label: 'Technical Support', value: 'support' },
    { label: 'Sales & Pricing', value: 'sales' },
    { label: 'Partnership', value: 'partnership' },
    { label: 'Feedback', value: 'feedback' },
    { label: 'Other', value: 'other' },
  ];

  faqs: FAQ[] = [
    {
      question: 'How do I register my organization?',
      answer:
        'Click on "Sign Up" and select "Register as HR Manager". Fill in your organization details and complete the verification process.',
    },
    {
      question: 'Is TalentBridge free to use?',
      answer:
        'We offer different pricing plans. Candidates can use basic features for free. Organizations have access to a free trial period.',
    },
    {
      question: 'How secure is the testing system?',
      answer:
        'We use unique access tokens, time tracking, and secure storage. All tests are monitored with complete audit trails.',
    },
    {
      question: 'Can I customize tests for my organization?',
      answer:
        'Yes! HR Managers can create custom tests with 6 different question types and set their own passing criteria.',
    },
  ];

  constructor(private fb: FormBuilder, private message: NzMessageService) {
    this.contactForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      inquiryType: ['general', Validators.required],
      subject: ['', [Validators.required, Validators.minLength(5)]],
      message: ['', [Validators.required, Validators.minLength(20)]],
    });
  }

  submitForm(): void {
    if (this.contactForm.valid) {
      this.isSubmitting = true;

      setTimeout(() => {
        this.message.success(
          'Thank you! Your message has been sent successfully. We will get back to you soon.'
        );
        this.contactForm.reset({ inquiryType: 'general' });
        this.isSubmitting = false;
      }, 1500);

    } else {
      Object.values(this.contactForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
